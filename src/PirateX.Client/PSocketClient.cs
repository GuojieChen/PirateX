using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PirateX.Client.Command;
using PirateX.Client.Protocol;
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

        public long Ts
        {
            get { return (int)End.Subtract(Start).TotalMilliseconds; }
        }

        public DateTime LogTime { get; set; }

        public bool Ok { get; set; }

        public bool SendOk { get; set; }

        public string GroupName { get; set; }

        public ProcessLog()
        {
            LogTime = DateTime.Now;
        }
    }

    public class PSocketClient : IDisposable
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

        private Dictionary<string, IJsonExecutor> m_ExecutorDict = new Dictionary<string, IJsonExecutor>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, IJsonBroadcastExecutor> m_BroadcastExectorDict = new Dictionary<string, IJsonBroadcastExecutor>();

        /// <summary> 客户端种子
        /// </summary>
        private int _clientSeed;

        /// <summary> 数据包处理器
        /// </summary>
        public IPackageProcessor PackageProcessor { get; private set; }

        public string CurrentMethod { get; private set; }
        public DateTime SendTime { get; private set; }

        private bool sendOk { get; set; }

        public object CurrentReq { get; set; }

        /// <summary>
        /// 构造一个PSocket 有两种方式
        /// 
        /// 1 : ps://host:port/    普通的Scoket
        /// 2 : pss://host:port/   安全型Socket
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="packageProcessor"></param>
        /// <param name="receiveBufferSize"></param>
        public PSocketClient(string uri, int receiveBufferSize = 2048,int sendQueueSize = 3)
        {
            TcpClientSession client;

            if (uri.StartsWith(m_UriPrefix, StringComparison.OrdinalIgnoreCase))
                client = CreateClient(uri);
            else if (uri.StartsWith(m_SecureUriPrefix, StringComparison.OrdinalIgnoreCase))
                client = CreateSecureClient(uri);
            else
                throw new ArgumentException("Invalid uri", "uri");

            m_ExecutorDict.Add("Ping", new Ping());
            m_ExecutorDict.Add("NewSeed", new NewSeed());

            PackageProcessor = new DefaultPackageProcessor() { ZipEnable = true };
            m_StateCode = PSocketStateConst.None;

            client.ReceiveBufferSize = receiveBufferSize;
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
                var oData = Encoding.UTF8.GetString(DataPackage.Unpack(e.Data, PackageProcessor.ServerKeys));

                object response = null;

                var jObject = JObject.Parse(oData);

                if (OnReceiveMessage != null)
                    OnReceiveMessage(sender, new MsgEventArgs(oData, e.Data, jObject["B"] == null));

                sw.Stop();

                log.Resp = jObject;

                if (jObject["Code"] != null)
                {
                    if (OnServerError != null)
                        OnServerError(this, new PErrorEventArgs(
                            Convert.ToInt32(jObject["Code"]),
                            Convert.ToString(jObject["Msg"])));

                    log.Ok = false;

                    if (OnResponseProcessed != null)
                        OnResponseProcessed(this, log);
                }
                else if (jObject["B"] != null)
                {
                    var method = jObject["B"].ToString();
                    var data = jObject["D"].ToString();

                    var executor = GetBroadcastExecutor(method);
                    if (executor != null)
                    {
                        var type = executor.GetType();
                        var o = Activator.CreateInstance(type);

                        try
                        {
                            var methodexec = type.GetMethod("GetData",
                                BindingFlags.Instance | BindingFlags.Public);

                            response = methodexec.Invoke(o, new[] { data });

                            methodexec = type.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public);
                            methodexec.Invoke(o, new[] { this, response });
                        }
                        catch (Exception exc)
                        {
                            client_Error(this, new ErrorEventArgs(new Exception("Message handling exception", exc)));
                        }
                    }
                }
                else
                {
                    var method = jObject["C"].ToString();
                    var data = jObject["D"].ToString();

                    var executor = GetExecutor(method);

                    if (executor != null)
                    {
                        var type = executor.GetType();
                        var o = Activator.CreateInstance(type);

                        try
                        {
                            var methodexec = type.GetMethod("GetResponseInfo",
                                BindingFlags.Instance | BindingFlags.Public);

                            response = methodexec.Invoke(o, new[] { data });

                            methodexec = type.GetMethod("Excute", BindingFlags.Instance | BindingFlags.Public);
                            methodexec.Invoke(o, new[] { this, response });
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

            return new AsyncTcpSession(targetEndPoint);
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

            return new SslStreamTcpSession(targetEndPoint);
        }

        #endregion

        public event EventHandler<MsgEventArgs> OnReceiveMessage;

        public event EventHandler<ErrorEventArgs> OnError;

        public event EventHandler<EventArgs> OnClosed;

        public event EventHandler<EventArgs> OnOpen;

        public event EventHandler<PErrorEventArgs> OnServerError;

        public event EventHandler<MsgEventArgs> OnSend;


        public delegate void OnResponseProcessedHandler(object sender, ProcessLog log);

        public event OnResponseProcessedHandler OnResponseProcessed;


        #region connect and close

        void OnConnected()
        {
            //CommandReader = ProtocolProcessor.CreateHandshakeReader(this);
            //ProtocolProcessor.SendHandshake(this);
            _clientSeed = Utils.GetTimestampAsSecond();
            var clientKey = new KeyGenerator(_clientSeed);
            PackageProcessor.ClientKeys.Add(clientKey.MakeKey());

            Send("NewSeed", new
            {
                Seed = _clientSeed,
                Format = "JSON"
            });
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
                if(Client!=null)
                    Client.Close();
            }
        }
        #endregion

        IJsonExecutor GetExecutor(string name)
        {
            string key = name;

            lock (m_ExecutorDict)
            {
                IJsonExecutor executor;

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
            var cmds = assembly.GetTypes().Where(item => typeof(IJsonExecutor).IsAssignableFrom(item));
            foreach (var type in cmds)
                m_ExecutorDict.Add(type.Name, Activator.CreateInstance(type) as IJsonExecutor);
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

        /// <summary>
        /// 像服务端发送信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void Send(string name, object data, object ex = null, bool? r = false)
        {
            if (name == null)
                return;

            if (State == PSocketState.None)
                return;

            var message = DataPackage.Pack(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                C = name,
                D = data,
                Ex = ex,
                O = O++,
                R = r
            })), PackageProcessor.ClientKeys, false);

            if (Client != null && Client.IsConnected)
            {
                try
                {
                    ArraySegment<byte> ds = new ArraySegment<byte>(message);

                    sendOk = Client.TrySend(ds);
                    //Client.Send(message, 0, message.Length);

                    CurrentMethod = name;
                    SendTime = DateTime.Now;
                    CurrentReq = data;
                    if (OnSend != null)
                        OnSend(this, new MsgEventArgs(null, message));
                }
                catch (Exception exception)
                {
                    if (OnError != null)
                        OnError(this, new ErrorEventArgs(exception));
                }
            }
        }

        public void Send<T>(T data)
        {
            Send(typeof(T).Name, data);
        }

        public void Open()
        {
            m_StateCode = PSocketStateConst.Connecting;
            if (Client != null)
                Client.Connect();
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
