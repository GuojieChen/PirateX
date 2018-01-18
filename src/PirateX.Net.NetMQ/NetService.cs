using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using PirateX.Core;
using PirateX.Core.Net;
using PirateX.Core.Utils;
using PirateX.Protocol;
using PirateX.Protocol.Package;

namespace PirateX.Net.NetMQ
{
    public class NetService : INetService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private INetManager NetSend { get; set; }
        
        private SubscriberSocket _subscriberSocket;

        public string PublisherSocketString { get; set; }
        public string ResponseHostString { get; set; }

        public TimeSpan DefaultTimeOuTimeSpan = new TimeSpan(0,0,5);
        
        private NetMQPoller Poller;

        private bool IsSetuped { get; set; }
        public virtual void Setup(INetManager netManager)
        {
            if(string.IsNullOrEmpty(ResponseHostString))
                throw new ArgumentNullException(nameof(ResponseHostString));

            _subscriberSocket = new SubscriberSocket(PublisherSocketString);

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

            var msg = e.Socket.ReceiveFrameBytes();//TryReceiveMultipartMessage();

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
        public virtual byte[] ProcessRequest(IProtocolPackage protocolPackage, byte[] body)
        {
            //REQ --- Router/Dealer --- REP

            if (protocolPackage == null)
                return null ;
            if(Logger.IsDebugEnabled)
                Logger.Debug($"request from {protocolPackage.RemoteEndPoint}");

            if (protocolPackage.CryptoByte > 0)
            {
                var last = NetSend.GetProtocolPackage(protocolPackage.Rid);
                if (!Equals(last.Id, protocolPackage.Id))
                {
                    protocolPackage.Close();

                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"!Equals(last.Id, protocolPackage.Id)");
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
            };
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                din.Profile.Add("_tin_", $"{DateTime.UtcNow.Ticks}");

            return RequestToRemoteResponseSocket(din);
        }

        private byte[] RequestToRemoteResponseSocket(In din)
        {
            try
            {
                //向远端Rep 服务器请求并获取数据
                using (var req = new RequestSocket(ResponseHostString) { Options = { } })
                {
                    req.SendFrame(din.ToProtobuf());
                    if (req.TryReceiveFrameBytes(DefaultTimeOuTimeSpan, out var responseBytes))
                        return responseBytes;
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

            //加入队列
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
