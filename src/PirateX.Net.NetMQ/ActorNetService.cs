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
using PirateX.Protocol.Package;

namespace PirateX.Net.NetMQ
{
    public class ActorNetService : IActorNetService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ActorConfig config;

        private PullSocket PullSocket { get; set; }
        private PushSocket PushSocket { get; set; }

        private NetMQPoller Poller { get; set; }

        private readonly NetMQQueue<byte[]> MessageQueue = new NetMQQueue<byte[]>();

        private IActorService _actorService;
        public ActorNetService(IActorService actorService, ActorConfig config)
        {
            this._actorService = actorService;
            _actorService.NetService = this;
            this.config = config;
        }

        /// <summary>
        /// 多线程处理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e)
        {
            var bytes = e.Socket.ReceiveFrameBytes();

            Logger.Debug($"ProcessTaskPullSocket1 {Thread.CurrentThread.ManagedThreadId} - {Thread.CurrentThread.IsThreadPoolThread}");

            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Logger.Debug($"ProcessTaskPullSocket2 {Thread.CurrentThread.ManagedThreadId} - {Thread.CurrentThread.IsThreadPoolThread}");

                try
                {
                    var msg = (byte[])obj;

                    var din = msg.FromProtobuf<In>();

                    if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                        din.Profile.Add("_itin_", $"{DateTime.UtcNow.Ticks}");

                    var context = new ActorContext()
                    {
                        Version = din.Version,//版本号
                        Action = (byte)din.Action,
                        Request = new PirateXRequestInfo(
                            din.HeaderBytes, //信息头
                            din.QueryBytes)//信息体
                        ,
                        RemoteIp = din.Ip,
                        LastNo = din.LastNo,
                        SessionId = din.SessionId,
                        Profile = din.Profile,
                        ServerName = din.ServerName,
                        ServerItmes = din.Items
                    };

                    _actorService.OnReceive(context);
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }

            }, bytes);
        }

        protected void EnqueueMessage(byte[] message)
        {
            MessageQueue.Enqueue(message);
        }

        public virtual void Start()
        {
            PullSocket = new PullSocket(config.PullSocketString);
            PullSocket.ReceiveReady += ProcessTaskPullSocket;

            PushSocket = new PushSocket(config.PushsocketString);
            Poller = new NetMQPoller() { MessageQueue, PullSocket };

            MessageQueue.ReceiveReady += (sender, args) =>
            {
                PushSocket.SendFrame(args.Queue.Dequeue());
            };

            _actorService.Start();
            Poller.RunAsync();
        }

        public virtual void Stop()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("ActorNetService Stopping...");

            Poller?.Stop();

            PullSocket?.Close();
            _actorService.Stop();
        }

        public void PushMessage(int rid, NameValueCollection headers, byte[] body)
        {
            EnqueueMessage(new Out()
            {
                Version = 1,
                Action = Action.Push,
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

        public void Seed(ActorContext context, NameValueCollection header, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, byte[] body)
        {
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                context.Profile.Add("_itout_", $"{DateTime.UtcNow.Ticks}");

            EnqueueMessage(new Out()
            {
                Version = 1,
                Action = Action.Seed,
                SessionId = context.SessionId,
                LastNo = context.Request.O,
                HeaderBytes = GetHeaderBytes(header),
                BodyBytes = body,
                Id = context.Token.Rid,

                ClientKeys = clientkeys,
                ServerKeys = serverkeys,
                Crypto = cryptobyte,

                Profile = context.Profile
            }.ToProtobuf());
        }

        public void SendMessage(ActorContext context, NameValueCollection header, byte[] body)
        {
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
            {
                if(!context.Profile.ContainsKey("_itout_"))
                    context.Profile.Add("_itout_", $"{DateTime.UtcNow.Ticks}");
            }

            EnqueueMessage(new Out()
            {
                Version = 1,
                Action = Action.Req,
                LastNo = context.Request.O,
                HeaderBytes = GetHeaderBytes(header),
                BodyBytes = body,
                Id = context.Token.Rid,
                Profile = context.Profile

            }.ToProtobuf());
        }
    }
}
