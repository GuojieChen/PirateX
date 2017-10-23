using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PirateX.Client.Command;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;
using SuperSocket.ClientEngine;
using ErrorEventArgs = System.IO.ErrorEventArgs;

namespace PirateX.Client
{
    public class ProcessLog
    {
        public int Rid { get; set; }

        public string Name { get; set; }

        public object Req { get; set; }

        public object Resp { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public long Ts => (int)End.Subtract(Start).TotalMilliseconds;

        public DateTime LogTime { get; set; }

        public bool Ok { get; set; }

        public bool SendOk { get; set; }

        public string GroupName { get; set; }

        public ProcessLog()
        {
            LogTime = DateTime.Now;
        }
    }

    public class PirateXClient : IDisposable
    {
        public int Id { get; set; }

        internal TcpClientSession Client { get; private set; }

        /// <summary> 自动ping的频率
        /// </summary>
        public int AutoSendPingInterval { get; set; }

        /// <summary>
        /// It is used for ping/pong and closing handshake checking
        /// </summary>
        private Timer m_WebSocketTimer;

        public Uri TargetUri { get; private set; }

        private int O { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Lang => "zh-CN";

        public string DefaultFormat = "protobuf";

        public string Device => Guid.NewGuid().ToString();

        protected int m_StateCode;

        internal int StateCode
        {
            get { return m_StateCode; }
        }

        public PSocketState State
        {
            get { return (PSocketState)m_StateCode; }
        }

        private string m_LastPingRequest;

        private const string m_UriScheme = "ps";

        private const string m_UriPrefix = m_UriScheme + "://";
        private const int m_defaultPort = 2012;

        private const string m_SecureUriScheme = "pss";
        private const int m_SecurePort = 443;

        private const string m_SecureUriPrefix = m_SecureUriScheme + "://";

        private Dictionary<string, IPirateXClientExecutor> m_ExecutorDict = new Dictionary<string, IPirateXClientExecutor>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, IJsonBroadcastExecutor> m_BroadcastExectorDict = new Dictionary<string, IJsonBroadcastExecutor>();

        /// <summary> 客户端种子
        /// </summary>
        private int _clientSeed;

        /// <summary> 数据包处理器
        /// </summary>
        public IProtocolPackage PackageProcessor { get; private set; }

        public string CurrentMethod { get; private set; }
        public DateTime SendTime { get; private set; }

        private EndPoint TargetEndPoint { get; set; }

        private bool sendOk { get; set; }

        public object CurrentReq { get; set; }

        private string _token;


        private IDictionary<string, IResponseConvert> _responseConverts = new Dictionary<string, IResponseConvert>();


        public NameValueCollection ExHeaders = new NameValueCollection();

        /// <summary>
        /// 构造一个PSocket 有两种方式
        /// 
        /// 1 : ps://host:port/    普通的Scoket
        /// 2 : pss://host:port/   安全型Socket
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="token"></param>
        /// <param name="sendQueueSize"></param>
        public PirateXClient(string uri, string token = "", int sendQueueSize = 3)
        {
            this._token = token;

            TcpClientSession client;

            if (uri.StartsWith(m_UriPrefix, StringComparison.OrdinalIgnoreCase))
                client = CreateClient(uri);
            else if (uri.StartsWith(m_SecureUriPrefix, StringComparison.OrdinalIgnoreCase))
                client = CreateSecureClient(uri);
            else
                throw new ArgumentException("Invalid uri", "uri");

            m_ExecutorDict.Add("Ping", new Ping());
            m_ExecutorDict.Add("NewSeed", new NewSeed());

            foreach (var responseConvert in typeof(IResponseConvert).Assembly.GetTypes().Where(item => typeof(IResponseConvert).IsAssignableFrom(item)))
            {
                if (responseConvert.IsInterface)
                    continue;

                var attrs = responseConvert.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                if (attrs.Any())
                {
                    var convertName = ((DisplayColumnAttribute)attrs[0]).DisplayColumn;
                    if (!string.IsNullOrEmpty(convertName))
                        _responseConverts.Add(convertName.ToLower(), (IResponseConvert)Activator.CreateInstance(responseConvert));
                }
            }


            PackageProcessor = new ProtocolPackage();  //new DefaultPackageProcessor() { ZipEnable = true };
            m_StateCode = PSocketStateConst.None;

            client.ReceiveBufferSize = 1024*10*100;
            client.Connected += new EventHandler(client_Connected);

            client.Closed += new EventHandler(client_Closed);
            client.Error += (sender, args) => { };
            client.DataReceived += new EventHandler<DataEventArgs>(client_DataReceived);

            Client = client;

            AutoSendPingInterval = -1; //-1 表示启用自动ping
        }


        void client_DataReceived(object sender, DataEventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var log = new ProcessLog()
            {
                Rid = Id,
                Name = CurrentMethod,
                Req = CurrentReq,
                Ok = true,
                SendOk = sendOk,
                Start = SendTime,
                End = DateTime.Now
            };

            try
            {
                var responsePackage = PackageProcessor.UnPackToPacket(e.Data);
                var responseInfo = new PirateXResponseInfo(responsePackage);

                var header = responseInfo.Headers;
                sw.Stop();

                log.Resp = header;


                if (Equals(header["i"], "2"))
                {
                    var method = header["c"];
                    var executor = GetBroadcastExecutor(method);
                    if (executor != null)
                    {
                        executor.Header = header;

                        var type = executor.GetType();
                        var o = (IPirateXClientExecutor)Activator.CreateInstance(type);

                        o.Header = header;
                        o.ResponseConvert = _responseConverts[responseInfo.Headers["format"] ?? DefaultFormat];
                        try
                        {
                            o.Excute(this, responsePackage.ContentBytes);
                        }
                        catch (Exception exc)
                        {
                            client_Error(this, new ErrorEventArgs(new Exception("Message handling exception", exc)));
                        }
                    }

                    if (OnNotified != null)
                        OnNotified(this, new MsgEventArgs(responseInfo.Headers["c"], responseInfo));
                }
                else if (Equals(header["i"], "1"))
                {
                    if (!Equals(header["code"], "200"))
                    {
                        if (OnServerError != null)
                            OnServerError(this, new PErrorEventArgs(
                                Convert.ToInt32(header["errorCode"]),
                                HttpUtility.UrlDecode(Convert.ToString(header["errorMessage"]))));

                        log.Ok = false;

                    }
                    else
                    {
                        var method = header["c"].ToString();

                        var executor = GetExecutor(method);

                        if (executor != null)
                        {
                            var type = executor.GetType();
                            var o = (IPirateXClientExecutor)Activator.CreateInstance(type);

                            o.Header = header;
                            o.ResponseConvert = _responseConverts[responseInfo.Headers["format"] ?? DefaultFormat];

                            try
                            {
                                o.Excute(this, responsePackage.ContentBytes);
                            }
                            catch (Exception exc)
                            {
                                client_Error(this, new ErrorEventArgs(new Exception("Message handling exception", exc)));
                            }

                            if (type == typeof(NewSeed))
                            {
                                m_StateCode = PSocketStateConst.Open;
                                if (OnOpen != null)
                                    OnOpen(this, new EventArgs());
                            }
                        }


                        if (OnReceiveMessage != null)
                            OnReceiveMessage(this, new MsgEventArgs(responseInfo.Headers["c"], responseInfo));
                    }

                    if (OnResponseProcessed != null)
                        OnResponseProcessed(this, log);

                }
            }
            catch (Exception exc)
            {
                client_Error(this, new ErrorEventArgs(exc));
            }
        }

        #region crate client
        private EndPoint ResolveUri(string uri, int defaultPort, out int port)
        {
            TargetUri = new Uri(uri);

            IPAddress ipAddress;

            EndPoint remoteEndPoint;

            port = TargetUri.Port;

            if (port <= 0)
                port = defaultPort;

            if (IPAddress.TryParse(TargetUri.Host, out ipAddress))
                remoteEndPoint = new IPEndPoint(ipAddress, port);
            else
                remoteEndPoint = new DnsEndPoint(TargetUri.Host, port);

            return remoteEndPoint;
        }

        TcpClientSession CreateClient(string uri)
        {
            int port;
            var targetEndPoint = ResolveUri(uri, m_defaultPort, out port);

            TargetEndPoint = targetEndPoint;

            return new AsyncTcpSession()
            {
            };
        }
        TcpClientSession CreateSecureClient(string uri)
        {
            int hostPos = uri.IndexOf('/', m_SecureUriPrefix.Length);

            if (hostPos < 0)//wss://localhost or wss://localhost:xxxx
            {
                hostPos = uri.IndexOf(':', m_SecureUriPrefix.Length, uri.Length - m_SecureUriPrefix.Length);

                if (hostPos < 0)
                    uri = uri + ":" + m_SecurePort + "/";
                else
                    uri = uri + "/";
            }
            else if (hostPos == m_SecureUriPrefix.Length)//wss://
            {
                throw new ArgumentException("Invalid uri", "uri");
            }
            else//wss://xxx/xxx
            {
                int colonPos = uri.IndexOf(':', m_SecureUriPrefix.Length, hostPos - m_SecureUriPrefix.Length);

                if (colonPos < 0)
                {
                    uri = uri.Substring(0, hostPos) + ":" + m_SecurePort + uri.Substring(hostPos);
                }
            }

            int port;
            var targetEndPoint = ResolveUri(uri, m_SecurePort, out port);

            TargetEndPoint = targetEndPoint;
            return new SslStreamTcpSession()
            {
            };
        }

        #endregion

        public event EventHandler<MsgEventArgs> OnNotified;


        public event EventHandler<MsgEventArgs> OnReceiveMessage;

        public event EventHandler<ErrorEventArgs> OnError;

        public event EventHandler<EventArgs> OnClosed;

        public event EventHandler<EventArgs> OnOpen;

        public event EventHandler<PErrorEventArgs> OnServerError;

        public event EventHandler<OutMsgEventArgs> OnSend;


        public delegate void OnResponseProcessedHandler(object sender, ProcessLog log);

        public event OnResponseProcessedHandler OnResponseProcessed;

        #region connect and close

        void OnConnected()
        {
            //CommandReader = ProtocolProcessor.CreateHandshakeReader(this);
            //ProtocolProcessor.SendHandshake(this);
            _clientSeed = Utils.GetTimestampAsSecond();
            var clientKey = new KeyGenerator(_clientSeed);
            PackageProcessor.PackKeys = clientKey.MakeKey();

            Send("NewSeed", $"seed={_clientSeed}", new NameValueCollection() { { "format", DefaultFormat } });
        }

        private void ClearTimer()
        {
            if (m_WebSocketTimer != null)
            {
                m_WebSocketTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_WebSocketTimer.Dispose();
                m_WebSocketTimer = null;
            }
        }

        void client_Closed(object sender, EventArgs e)
        {
            var fireBaseClose = false;

            if (m_StateCode == PSocketStateConst.Closing || m_StateCode == PSocketStateConst.Open)
                fireBaseClose = true;

            m_StateCode = PSocketStateConst.Closed;

            if (fireBaseClose)
                ClearTimer();

            if (OnClosed != null)
                OnClosed(sender, e);
        }

        void client_Connected(object sender, EventArgs e)
        {
            OnConnected();
        }

        void client_Error(object sender, ErrorEventArgs e)
        {
            if (OnError != null)
                OnError(sender, e);

            //Also fire close event if the connection fail to connect
            if (m_StateCode == PSocketStateConst.Connecting)
            {
                m_StateCode = PSocketStateConst.Closing;
                if (Client != null)
                    Client.Close();
            }
        }
        #endregion

        IPirateXClientExecutor GetExecutor(string name)
        {
            string key = name;

            lock (m_ExecutorDict)
            {
                IPirateXClientExecutor executor;

                if (!m_ExecutorDict.TryGetValue(key, out executor))
                    return null;

                return executor;
            }
        }

        /// <summary>
        /// 获取广播处理对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IJsonBroadcastExecutor GetBroadcastExecutor(string name)
        {
            string key = name;

            lock (m_BroadcastExectorDict)
            {
                IJsonBroadcastExecutor executor;
                if (!m_BroadcastExectorDict.TryGetValue(key, out executor))
                    return null;
                return executor;
            }
        }
        /// <summary> 注册请求处理模块
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterExcutor(Assembly assembly)
        {
            var cmds = assembly.GetTypes().Where(item => typeof(IPirateXClientExecutor).IsAssignableFrom(item));
            foreach (var type in cmds)
                m_ExecutorDict.Add(type.Name, Activator.CreateInstance(type) as IPirateXClientExecutor);
        }

        /// <summary> 注册广播处理模块
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterBroadcastExcutor(Assembly assembly)
        {
            var cmds = assembly.GetTypes().Where(item => typeof(IJsonBroadcastExecutor).IsAssignableFrom(item));
            foreach (var type in cmds)
                m_BroadcastExectorDict.Add(type.Name, Activator.CreateInstance(type) as IJsonBroadcastExecutor);
        }

        public void Send(string name, string querystring, NameValueCollection exheader = null)
        {
            var headerNc = new NameValueCollection
            {
            };

            foreach (string key in ExHeaders.Keys)
                headerNc.Add(key, ExHeaders[key]);

            headerNc.Add("c", name);
            headerNc.Add("o", $"{O++}");
            headerNc.Add("t", $"{Utils.GetTimestamp()}");
            headerNc.Add("lang", $"{Lang}");
            headerNc.Add("format", "protobuf");
            headerNc.Add("token", HttpUtility.UrlEncode(_token));

            if (exheader != null)
            {
                foreach (var item in exheader.AllKeys)
                    headerNc[item] = exheader[item];
            }


            var reqeustInfo = new PirateXRequestInfo(headerNc, HttpUtility.ParseQueryString(querystring));

            var package = reqeustInfo.ToRequestPackage();
            var senddatas = PackageProcessor.PackPacketToBytes(package);
            Client.Send(senddatas, 0, senddatas.Length);
            SendTime = DateTime.Now;
            if (OnSend != null)
                OnSend(this, new OutMsgEventArgs(name, reqeustInfo));
        }

        public void Send<T>(T data)
        {
            //Send(typeof(T).Name, data);
        }

        public void Open()
        {
            m_StateCode = PSocketStateConst.Connecting;
            if (Client != null)
                Client.Connect(TargetEndPoint);
        }

        public virtual void Close()
        {
            Client.Close();
        }

        public void Dispose()
        {
            var client = Client;

            if (client != null)
            {
                if (client.IsConnected)
                    client.Close();

                Client = null;
            }
        }
    }

    public enum PSocketState : int
    {
        None = PSocketStateConst.None,
        Connecting = PSocketStateConst.Connecting,
        Open = PSocketStateConst.Open,
        Closing = PSocketStateConst.Closing,
        Closed = PSocketStateConst.Closed
    }

    public static class PSocketStateConst
    {
        public const int None = -1;

        public const int Connecting = 0;

        public const int Open = 1;

        public const int Closing = 2;

        public const int Closed = 3;
    }
}
