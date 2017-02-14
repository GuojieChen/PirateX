using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Protocol.Package;

namespace PirateX.Net
{
    public class NetService
    {
        private INetSend NetSend { get; set; }

        /// <summary>
        /// 下发任务
        /// </summary>
        private PushSocket sender;

        /// <summary>
        /// 上报配置和结果
        /// </summary>
        private PullSocket responseSocket;

        public NetMQQueue<NetMQMessage> PushQueue = new NetMQQueue<NetMQMessage>();

        private NetMQPoller Poller;

        private bool IsSetuped { get; set; }

        public virtual void Setup(string pushsocket, string pullsocket, INetSend netSend)
        {
            if (string.IsNullOrEmpty(pushsocket))
                throw new ArgumentNullException(nameof(pushsocket));
            if (string.IsNullOrEmpty(pullsocket))
                throw new ArgumentNullException(nameof(pullsocket));
            if (netSend == null)
                throw new ArgumentNullException(nameof(netSend));

            sender = new PushSocket(pushsocket);
            responseSocket = new PullSocket(pullsocket);

            Poller = new NetMQPoller()
            {
                sender,
                responseSocket,
                PushQueue
            };

            NetSend = netSend;

            PushQueue.ReceiveReady += (o, args) =>
            {
                var msg = PushQueue.Dequeue();
                sender.SendMultipartMessage(msg);
            };

            responseSocket.ReceiveReady += ProcessResponse;

            IsSetuped = true;
        }

        //服务器向客户端下发数据
        public virtual void ProcessResponse(object o, NetMQSocketEventArgs e)
        {
            var msg = responseSocket.ReceiveMultipartMessage();
            //msg[0].Buffer //版本号
            var action = msg[1].ConvertToString();
            var sessionid = msg[2].ConvertToString();

            var clientkey = msg[3].Buffer;
            var serverkey = msg[4].Buffer;

            var header = msg[5].Buffer;
            var content = msg[6].Buffer;

            var response = new PirateXResponsePackage()
            {
                HeaderBytes = header,
                ContentBytes = content
            };

            var protocolPackage = NetSend.GetProtocolPackage(sessionid);

            //将消息下发到客户端
            if (protocolPackage == null)
                return;
            if (protocolPackage.ClientKeys == null)
                protocolPackage.ClientKeys = clientkey;
            if (protocolPackage.ServerKeys == null)
                protocolPackage.ServerKeys = serverkey;

            var bytes = protocolPackage.PackResponsePackageToBytes(response);

            NetSend.Send(sessionid, bytes);
        }

        /// <summary>
        /// 收到客户端请求，交由Actor进行处理
        /// </summary>
        /// <param name="protocolPackage"></param>
        /// <param name="body"></param>
        public virtual void ProcessRequest(ProtocolPackage protocolPackage, byte[] body)
        {
            if (protocolPackage == null)
                return;

            var request = protocolPackage.UnPackToRequestPackage(body);

            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append("action");//动作
            msg.Append(protocolPackage.SessionID);//sessionid
            msg.Append(protocolPackage.ClientKeys);//客户端密钥
            msg.Append(protocolPackage.ServerKeys);//服务端密钥
            msg.Append(request.HeaderBytes);//信息头
            msg.Append(request.ContentBytes);//信息体
            //加入队列
            PushQueue.Enqueue(msg);
        }

        public virtual void Start()
        {
            if (!IsSetuped)
                throw new ApplicationException("Please Setup firset!");

            Poller.RunAsync();
        }

        public virtual void Stop()
        {
            Poller.Stop();
        }
    }
}
