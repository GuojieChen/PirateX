using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
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
    public class ActorNetService : IActorNetService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ActorConfig config;

        private ResponseSocket ResponseSocket { get; set; }

        private PublisherSocket PublisherSocket { get; set; }

        private NetMQPoller Poller { get; set; }

        private IActorService _actorService;

        public ActorNetService(IActorService actorService, ActorConfig config)
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
            var bytes = e.Socket.ReceiveFrameBytes();

            Logger.Debug($"ProcessRequest {Thread.CurrentThread.ManagedThreadId} - {Thread.CurrentThread.IsThreadPoolThread}");

            //ThreadPool.QueueUserWorkItem((obj) =>
            //{

            byte[] response = null; 
            try
            {
                var msg = (byte[]) bytes;

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
                Logger.Error(exception);
            }
            finally
            {
                if(response!=null)
                    e.Socket.SendFrame(response);
                else
                    e.Socket.SendFrameEmpty();
            }

            //}, bytes);
        }

        public virtual void Start()
        {
            ResponseSocket = new ResponseSocket(config.ResponseSocketString);
            ResponseSocket.ReceiveReady += ProcessRequest;

            Poller = new NetMQPoller() { ResponseSocket };

            _actorService.Start();
            Poller.RunAsync();
        }

        public virtual void Stop()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("ActorNetService Stopping...");

            Poller?.Stop();
            ResponseSocket?.Close();
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
