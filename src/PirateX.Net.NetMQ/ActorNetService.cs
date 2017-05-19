using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core;
using PirateX.Core.Actor;
using PirateX.Core.Net;
using PirateX.Core.Session;
using PirateX.Core.Utils;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;

namespace PirateX.Net.NetMQ
{
    public class ActorNetService : IActorNetService
    {
        private ActorConfig config;

        private PullSocket PullSocket { get; set; }
        private PushSocket PushSocket { get; set; }

        private NetMQPoller Poller { get; set; }

        private NetMQPoller PullPoller { get; set; }

        private readonly NetMQQueue<byte[]> MessageQueue = new NetMQQueue<byte[]>();

        private IActorService _actorService;
        public ActorNetService(IActorService actorService, ActorConfig config)
        {
            this._actorService = actorService;
            _actorService.NetService = this;
            this.config = config;
        }

        private void SetUp()
        {
            _actorService.Setup();

            PullSocket = new PullSocket(config.PullSocketString);
            PullSocket.ReceiveReady += ProcessTaskPullSocket;

            PushSocket = new PushSocket(config.PushsocketString);
            Poller = new NetMQPoller() { PushSocket, MessageQueue };
            PullPoller = new NetMQPoller(){ PullSocket };

            MessageQueue.ReceiveReady += (sender, args) =>
            {
                PushSocket.SendFrame(args.Queue.Dequeue());
            };

        }

        /// <summary>
        /// 多线程处理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e)
        {
            var bytes = e.Socket.ReceiveFrameBytes();

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    var msg = (byte[])state;

                    var din = msg.FromProtobuf<In>();
                    

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
                        SessionId = din.SessionId
                    };

#if PERFORM
                    context.Request.Headers.Add("_itin_", $"{DateTime.UtcNow.Ticks}");

                    new ProfilerLog()
                    {
                        Token = context.Request.Token,
                        Ip = context.RemoteIp,
                        Tin = new Ticks()
                        {
                            Start = Convert.ToInt64(context.Request.Headers["_tin_"]),
                            End = Convert.ToInt64(context.Request.Headers["_itin_"])
                        }
                    }.Log();

#endif
                    _actorService.OnReceive(context);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            },bytes);
        }

        private void CallActor(object obj)
        {
            var context = obj as ActorContext;
            if (context != null)
            {
                _actorService.OnReceive(context);
            }
            else
            {
                throw new Exception("call actor param type error");
            }
        }

        protected void EnqueueMessage(byte[] message)
        {
            MessageQueue.Enqueue(message);
        }

        public virtual void Start()
        {
            SetUp();

            _actorService.Start();
            Poller.RunAsync();
            PullPoller.RunAsync();
        }

        public virtual void Stop()
        {
            Poller?.Stop();
            PullPoller?.Stop();

            PullSocket?.Close();
            _actorService.Stop();
        }

        public void PushMessage(int rid, NameValueCollection headers, byte[] body)
        {
            //var repMsg = new NetMQMessage();
            //repMsg.Append(new byte[] { 1 });//版本号
            //repMsg.Append(new byte[] { (byte)Action.Req });//动作
            //repMsg.Append("");
            //repMsg.Append(-1);
            //repMsg.Append(GetHeaderBytes(headers));//信息头
            //if (body != null)
            //    repMsg.Append(body);//信息体
            //else
            //    repMsg.AppendEmptyFrame();

            //repMsg.Append(rid);



            EnqueueMessage(new Out()
            {
                Version = 1,
                Action = Action.Req,
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

        public void Seed(ActorContext context, NameValueCollection header, byte cryptobyte , byte[] clientkeys,byte[] serverkeys , byte[] body)
        {
            //var repMsg = new NetMQMessage();
            //repMsg.Append(new byte[] { context.Version });//版本号
            //repMsg.Append(new byte[] { (byte)Action.Seed });//动作
            //repMsg.Append(context.SessionId);//sessionid
            //repMsg.Append(context.Request.O);
            //repMsg.Append(GetHeaderBytes(header));//信息头
            //repMsg.Append(body);
            //repMsg.Append(context.Token.Rid);

            //repMsg.Append(clientkeys);//客户端密钥
            //repMsg.Append(serverkeys);//服务端密钥
            //repMsg.Append(new byte[] { cryptobyte });//加密项
            //repMsg.Append(context.Token.Rid);

            //EnqueueMessage(repMsg);



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
            }.ToProtobuf());
        }


        public void SendMessage(ActorContext context, NameValueCollection header, byte[] body)
        {

#if PERFORM
            header.Add("_itout_", $"{DateTime.UtcNow.Ticks}");


            new ProfilerLog()
            {
                Token = context.Request.Token,
                Ip = context.RemoteIp,
                iTin = new Ticks()
                {
                    Start = Convert.ToInt64(context.Request.Headers["_itin_"]),
                    End = Convert.ToInt64(context.Request.Headers["_itout_"])
                }
            }.Log();

#endif

            EnqueueMessage(new Out()
            {
                Version = 1,
                Action = Action.Req,
                LastNo = context.Request.O,
                HeaderBytes = GetHeaderBytes(header),
                BodyBytes = body,
                Id = context.Token.Rid,

            }.ToProtobuf());
        }
    }
}
