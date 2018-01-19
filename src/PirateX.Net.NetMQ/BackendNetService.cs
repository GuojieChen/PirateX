using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using PirateX.Core;
using PirateX.Core.Actor;
using PirateX.Core.Net;
using PirateX.Core.Session;
using PirateX.Core.Utils;
using PirateX.Protocol;
using PirateX.Protocol.Package;

namespace PirateX.Net.NetMQ
{
    public class BackendNetService : IActorNetService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ActorConfig config;

        private PublisherSocket PublisherSocket { get; set; }

        private NetMQPoller Poller { get; set; }

        private IActorService _actorService;

        public BackendNetService(IActorService actorService, ActorConfig config)
        {
            this._actorService = actorService;
            _actorService.ActorNetService = this;
            this.config = config;
        }

        /// <summary>
        /// 多线程处理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProcessRequest(object sender, NetMQSocketEventArgs e)
        {
            //  将消息中空帧之前的所有内容（信封）保存起来，
            //  本例中空帧之前只有一帧，但可以有更多。
            var address = e.Socket.ReceiveFrameString();
            //Console.WriteLine("[B] Message received: {0}", address);
            var empty = e.Socket.ReceiveFrameString();
            //Console.WriteLine("[B] Message received: {0}", empty);
            var msg = e.Socket.ReceiveFrameBytes();

            byte[] response = null; 
            try
            {
                var din = msg.FromProtobuf<In>();

                if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                    din.Profile.Add("_itin_", $"{DateTime.UtcNow.Ticks}");

                var context = new ActorContext()
                {
                    Version = din.Version, //版本号
                    Action = (byte) din.Action,
                    Request = new PirateXRequestInfo(
                        din.HeaderBytes, //信息头
                        din.QueryBytes) //信息体
                    ,
                    RemoteIp = din.Ip,
                    LastNo = din.LastNo,
                    SessionId = din.SessionId,
                    Profile = din.Profile,
                    ServerName = din.ServerName,
                    ServerItmes = din.Items
                };

                response = _actorService.OnReceive(context);
            }
            catch (Exception exception)
            {
                //TODO 处理，，，
                Logger.Error(exception);
            }
            finally
            {
                e.Socket.SendMoreFrame(address);
                e.Socket.SendMoreFrame("");
                if (response != null)
                    e.Socket.SendFrame(response);
                else
                    e.Socket.SendFrame("");
            }
            //TODO 需要处理 finnaly的异常，感知socket 然后重启
        }

        private LRUBroker _broker = null;
        private List<RequestSocket> Workers = new List<RequestSocket>(); 
        public virtual void Start()
        {
            var connectto = config.ResponseSocketString;
            //在检测到 ResponseSocketString 是已 @开头的 自己内置一个LRUBroker
            if (config.ResponseSocketString.StartsWith("@"))
            {
                _broker = new LRUBroker(config.ResponseSocketString, "@tcp://localhost:3001");
                connectto = ">tcp://localhost:3001";
            }

            Poller = new NetMQPoller();

            for (int i = 0; i < 1; i++)
            {
                //这里可以多开几个
                var responseSocket = new RequestSocket(connectto);
                responseSocket.Options.Identity = Encoding.UTF8.GetBytes($"{Dns.GetHostName()}_{Process.GetCurrentProcess().Id}_{i}");
                responseSocket.ReceiveReady += ProcessRequest;
                Poller.Add(responseSocket);
                Workers.Add(responseSocket);
            }

            _actorService.Start();
            _broker.StartAsync();
            Poller.RunAsync();
            
            Thread.Sleep(200);
            foreach (var worker in Workers)
                worker.SendFrame(new byte[]{1});
        }

        public virtual void Stop()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("ActorNetService Stopping...");

            Poller?.Stop();
            foreach (var worker in Workers)
                worker.Close();

            _broker?.Stop();

            _actorService.Stop();
        }
        /// <summary>
        /// 采用订阅发布的方式,向所有网关推送消息
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        public void PushMessage(int rid, NameValueCollection headers, byte[] body)
        {
            PublisherSocket.SendFrame(new Out()
            {
                Version = 1,
                Action = PirateXAction.Push,
                LastNo = -1,
                HeaderBytes = GetHeaderBytes(headers),
                BodyBytes = body,
                Id = rid,
            }.ToProtobuf());
        }

        private byte[] GetHeaderBytes(NameValueCollection headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a])));
        }

        public byte[] Seed(ActorContext context, NameValueCollection header, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, byte[] body)
        {
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                context.Profile.Add("_itout_", $"{DateTime.UtcNow.Ticks}");

            return new Out()
            {
                Version = 1,
                Action = PirateXAction.Seed,
                SessionId = context.SessionId,
                LastNo = context.Request.O,
                HeaderBytes = GetHeaderBytes(header),
                BodyBytes = body,
                Id = context.Token.Rid,

                ClientKeys = clientkeys,
                ServerKeys = serverkeys,
                Crypto = cryptobyte,

                Profile = context.Profile
            }.ToProtobuf();
        }

        public byte[] SendMessage(ActorContext context, NameValueCollection header, byte[] body)
        {
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
            {
                if(!context.Profile.ContainsKey("_itout_"))
                    context.Profile.Add("_itout_", $"{DateTime.UtcNow.Ticks}");
            }

            return new Out()
            {
                Version = 1,
                Action = PirateXAction.Req,
                LastNo = context.Request.O,
                HeaderBytes = GetHeaderBytes(header),
                BodyBytes = body,
                Id = context.Token.Rid,
                Profile = context.Profile

            }.ToProtobuf();
        }
    }
}
