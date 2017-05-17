using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core.Actor;
using PirateX.Core.Net;
using PirateX.Core.Session;
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
        private readonly NetMQQueue<NetMQMessage> MessageQueue = new NetMQQueue<NetMQMessage>();


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
            Poller = new NetMQPoller() { PullSocket, PushSocket, MessageQueue };

            MessageQueue.ReceiveReady += (sender, args) =>
            {
                PushSocket.SendMultipartMessage(args.Queue.Dequeue());
            };
        }

        /// <summary>
        /// 多线程处理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e)
        {
#if PERFORM
            var t1 = DateTime.UtcNow.Ticks;
#endif

            var msg = e.Socket.ReceiveMultipartMessage();
            var version = msg[0].Buffer[0];
            var action = msg[1].Buffer[0];

            if (action == 0)
                return;


            var context = new ActorContext()
            {
                Version = version,//版本号
                Action = action,
                Request = new PirateXRequestInfo(
                    msg[2].Buffer, //信息头
                    msg[3].Buffer)//信息体
                ,
                //ResponseCovnert = "protobuf",
                RemoteIp = msg[4].ConvertToString(),
                LastNo = msg[5].ConvertToInt32(),
                SessionId = msg[6].ConvertToString()
            };

#if PERFORM
            context.Request.Headers.Add("_itin_1_", $"{t1}");
            context.Request.Headers.Add("_itin_", $"{DateTime.UtcNow.Ticks}");
#endif
            _actorService.OnReceive(context);
            //            ThreadPool.QueueUserWorkItem(new WaitCallback(CallActor), context);
            /*Task.Factory.StartNew(() => _actorService.OnReceive(context)).ContinueWith(t =>
            {
                //发生内部错误

                //发生异常需要处理
                //t.Exception
            }, TaskContinuationOptions.OnlyOnFaulted);*/

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

        protected void EnqueueMessage(NetMQMessage message)
        {
            MessageQueue.Enqueue(message);
        }

        public virtual void Start()
        {
            SetUp();

            _actorService.Start();
            Poller.RunAsync();
        }

        public virtual void Stop()
        {
            Poller?.Stop();
            PullSocket?.Close();

            _actorService.Stop();
        }

        public void PushMessage(int rid, NameValueCollection headers, byte[] body)
        {
            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { 1 });//版本号
            repMsg.Append(new byte[] { (byte)Action.Req });//动作
            repMsg.Append("");
            repMsg.Append(-1);
            repMsg.Append(GetHeaderBytes(headers));//信息头
            if (body != null)
                repMsg.Append(body);//信息体
            else
                repMsg.AppendEmptyFrame();

            repMsg.Append(rid);

            EnqueueMessage(repMsg);
        }


        private byte[] GetHeaderBytes(NameValueCollection headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a])));
        }

        public void Seed(ActorContext context, NameValueCollection header, byte cryptobyte , byte[] clientkeys,byte[] serverkeys , byte[] body)
        {
            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { context.Version });//版本号
            repMsg.Append(new byte[] { (byte)Action.Seed });//动作
            repMsg.Append(context.SessionId);//sessionid
            repMsg.Append(context.Request.O);
            repMsg.Append(GetHeaderBytes(header));//信息头
            repMsg.Append(body);
            repMsg.Append(context.Token.Rid);

            repMsg.Append(clientkeys);//客户端密钥
            repMsg.Append(serverkeys);//服务端密钥
            repMsg.Append(new byte[] { cryptobyte });//加密项
            repMsg.Append(context.Token.Rid);

            EnqueueMessage(repMsg);
        }


        public void SendMessage(ActorContext context, NameValueCollection header, byte[] body)
        {

#if PERFORM
            header.Add("_itout_", $"{DateTime.UtcNow.Ticks}");
#endif

            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { context.Version });//版本号
            repMsg.Append(new byte[] { (byte)Action.Req });//动作
            repMsg.Append("");//sessionid
            repMsg.Append(context.Request.O);
            repMsg.Append(GetHeaderBytes(header));//信息头
            if (body != null)
                repMsg.Append(body);//信息体
            else
                repMsg.AppendEmptyFrame();
            repMsg.Append(context.Token.Rid);//sessionid

            EnqueueMessage(repMsg);
        }
    }
}
