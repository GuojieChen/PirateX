using System;
using System.Collections.Specialized;
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

            MessageQueue.ReceiveReady += (sender, args) => { PushSocket.SendMultipartMessage(args.Queue.Dequeue()); };
        }

        /// <summary>
        /// 多线程处理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e)
        {
            var msg = e.Socket.ReceiveMultipartMessage();
            var version = msg[0].Buffer[0];
            var action = msg[1].Buffer[0];

            if (action == 0)
                return;


            var context = new ActorContext()
            {
                Version = version,//版本号
                Action = action,
                SessionId = msg[2].ConvertToString(),//sessionid
                ClientKeys = msg[3].Buffer,//客户端密钥
                ServerKeys = msg[4].Buffer,//服务端密钥
                Request = new PirateXRequestInfo(
                    msg[5].Buffer, //信息头
                    msg[6].Buffer)//信息体
                ,
                ResponseCovnert = "protobuf",
                RemoteIp = msg[7].ConvertToString(),
                CryptoByte = msg[8].Buffer[0],
                LastNo = msg[9].ConvertToInt32(),
            };

#if DEBUG
            context.Request.Headers.Add("_itin_", $"{DateTime.UtcNow.Ticks}");
#endif


            Task.Factory.StartNew(() => _actorService.OnReceive(context)).ContinueWith(t =>
            {
                //发生内部错误

                //发生异常需要处理
                //t.Exception
            }, TaskContinuationOptions.OnlyOnFaulted);
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

        public void PushMessage(PirateSession role, NameValueCollection headers, byte[] body)
        {
            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { 1 });//版本号
            repMsg.Append(new byte[] { 1 });//动作
            repMsg.Append(role.SessionId);//sessionid
            repMsg.Append(role.ClientKeys);//客户端密钥
            repMsg.Append(role.ServerKeys);//服务端密钥
            repMsg.Append(new byte[] { role.CryptoByte });//加密项
            repMsg.Append(-1);
            repMsg.Append(GetHeaderBytes(headers));//信息头
            if (body != null)
                repMsg.Append(body);//信息体
            else
                repMsg.AppendEmptyFrame();

            EnqueueMessage(repMsg);
        }


        private byte[] GetHeaderBytes(NameValueCollection headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a])));
        }

        public void SendMessage(ActorContext context, NameValueCollection header, byte[] body)
        {

#if DEBUG
            header.Add("_itout_", $"{DateTime.UtcNow.Ticks}");
#endif

            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { context.Version });//版本号
            repMsg.Append(new byte[] { 1 });//动作
            repMsg.Append(context.SessionId);//sessionid
            repMsg.Append(context.ClientKeys);//客户端密钥
            repMsg.Append(context.ServerKeys);//服务端密钥
            repMsg.Append(new byte[] { context.CryptoByte });//加密项
            repMsg.Append(context.Request.O);
            repMsg.Append(GetHeaderBytes(header));//信息头
            if (body != null)
                repMsg.Append(body);//信息体
            else
                repMsg.AppendEmptyFrame();

            EnqueueMessage(repMsg);
        }
    }
}
