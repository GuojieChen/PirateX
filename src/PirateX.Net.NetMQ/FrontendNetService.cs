using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using PirateX.Core;
using PirateX.Protocol;

namespace PirateX.Net.NetMQ
{
    public class FrontendNetService : INetService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private INetManager NetSend { get; set; }
        
        private SubscriberSocket _subscriberSocket;
        /// <summary>
        /// 前段处理器标识
        /// 用以sub/pub 
        /// </summary>
        private string FrontendID = $"{Dns.GetHostName()}-{Process.GetCurrentProcess().Id}";

        public string PublisherSocketString { get; set; }
        public string ResponseHostString { get; set; }

        public TimeSpan DefaultTimeOuTimeSpan = TimeSpan.FromSeconds(5);
        
        private NetMQPoller Poller;

        private bool IsSetuped { get; set; }
        public virtual void Setup(INetManager netManager)
        {
            if(string.IsNullOrEmpty(ResponseHostString))
                throw new ArgumentNullException(nameof(ResponseHostString));

            _subscriberSocket = new SubscriberSocket(PublisherSocketString);
            _subscriberSocket.Subscribe(FrontendID);

            Poller = new NetMQPoller()
            {
                _subscriberSocket,
            };

            NetSend = netManager;

            _subscriberSocket.ReceiveReady += ProcessSubscribe;

            IsSetuped = true;
        }

        //处理订阅收到的数据,然后发送给指定的Client
        protected virtual void ProcessSubscribe(object o, NetMQSocketEventArgs e)
        {
            //TODO https://netmq.readthedocs.io/en/latest/poller/   #Performance

            if(Logger.IsTraceEnabled)
                Logger.Trace($"ProcessSubscribe -> ThreadID = {Thread.CurrentThread.ManagedThreadId}");

            var topic = e.Socket.ReceiveFrameBytes();//TryReceiveMultipartMessage();
            var msg = e.Socket.ReceiveFrameBytes();

            var dout = msg.FromProtobuf<Out>();

            var protocolPackage = NetSend.GetProtocolPackage(dout.Id);
            if (protocolPackage == null)
                return;

            var response = new PirateXResponsePackage()
            {
                HeaderBytes = dout.HeaderBytes,
                ContentBytes = dout.BodyBytes
            };

            var bytes = protocolPackage.PackPacketToBytes(response);
            protocolPackage.Send(bytes);
        }

        /// <summary>
        /// 解析token信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual IToken GetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;
            var odatas = Convert.FromBase64String(token);

            return odatas.FromProtobuf<Token>();
        }

        /// <summary>
        /// 收到客户端请求，通过 REQ/REP的模式向远端Actor请求处理,并获得数据
        /// </summary>
        /// <param name="protocolPackage"></param>
        /// <param name="body"></param>
        public virtual Out ProcessRequest(IProtocolPackage protocolPackage, byte[] body)
        {
            //LRU
            //REQ --- Router/Router --- REQ

            if (protocolPackage == null)
                return null ;
            if(Logger.IsDebugEnabled)
                Logger.Debug($"request from #{protocolPackage.Id}# {protocolPackage.RemoteEndPoint}");

            if (protocolPackage.CryptoByte > 0)
            {
                var last = NetSend.GetProtocolPackage(protocolPackage.Rid);
                if (!Equals(last.Id, protocolPackage.Id))
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Warn($"!Equals(last.Id, protocolPackage.Id)");

                    protocolPackage.Close();

                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"#{protocolPackage.Id}# !Equals(last.Id, protocolPackage.Id)");
                    return null ;
                }
            }
            //解密Cient数据包
            var request = protocolPackage.UnPackToPacket(body);

            var din = new In()
            {
                Version = 1,
                Action = PirateXAction.Req,
                HeaderBytes = request.HeaderBytes,
                QueryBytes = request.ContentBytes,
                Ip = (protocolPackage.RemoteEndPoint as IPEndPoint).Address.ToString(),
                LastNo = protocolPackage.LastNo,
                SessionId = protocolPackage.Id,
                FrontendID = FrontendID,
            };
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                din.Profile.Add("_tin_", $"{DateTime.UtcNow.Ticks}");

            return RequestToRemoteResponseSocket(din);
        }

        private Out RequestToRemoteResponseSocket(In din)
        {
            try
            {
                //向远端Rep 服务器请求并获取数据
                using (var req = new RequestSocket(ResponseHostString) { Options = { } })
                {
                    req.Options.Identity = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"));

                    req.SendFrame(din.ToProtobuf());

                    return req.ReceiveFrameBytes().FromProtobuf<Out>();

                    //if (req.TrySendFrame(TimeSpan.FromMilliseconds(200),din.ToProtobuf()))
                    //{
                    //    if (req.TryReceiveFrameBytes(DefaultTimeOuTimeSpan, out var responseBytes))
                    //        return responseBytes.FromProtobuf<Out>();
                    //    else
                    //    {
                    //        if (Logger.IsWarnEnabled)
                    //            Logger.Warn("request to remote TryReceive timeout");
                    //    }
                    //}
                    //else
                    //{
                    //    if (Logger.IsWarnEnabled)
                    //        Logger.Warn("request to remote TrySend timeout");
                    //}
                }
            }
            catch (Exception e)
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error(e);

                return null;
            }

            return null;
        }
        
        public virtual void Ping(int onlinecount)
        {
            RequestToRemoteResponseSocket(new In()
            {
                Version = 1,
                Action = PirateXAction.Ping,
                Items = new Dictionary<string, string>() { { "OnlineCount", $"{onlinecount}" } }
            });
        }

        public virtual void OnSessionClosed(IProtocolPackage protocolPackage)
        {
            if (protocolPackage == null)
                return;

            // 加入队列
            RequestToRemoteResponseSocket(new In()
            {
                Version = 1,
                Action = PirateXAction.Closed,
                Ip = (protocolPackage.RemoteEndPoint as IPEndPoint)?.Address.ToString(),
                LastNo = protocolPackage.LastNo,
                SessionId = protocolPackage.Id,
            });
        }

        public virtual void Start()
        {
            if (!IsSetuped)
                throw new ApplicationException("Please Setup firset!");

            Poller.RunAsync();
        }

        public virtual void Stop()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("Stopping...");

            if (Logger.IsDebugEnabled)
                Logger.Debug("Stopping Poller...");

            Poller?.StopAsync();
            Poller = null;
        }
    }
}
