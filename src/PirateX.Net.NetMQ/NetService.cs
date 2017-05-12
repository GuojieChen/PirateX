using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core.Net;
using PirateX.Protocol;
using PirateX.Protocol.Package;

namespace PirateX.Net.NetMQ
{
    public class NetService : INetService
    {
        private INetManager NetSend { get; set; }

        public string PushsocketString { get; set; }
        public string PullSocketString { get; set; }
        public string XPubSocketString { get; set; }
        public string XSubSocketString { get; set; }

        /// <summary>
        /// 下发任务
        /// </summary>
        private PushSocket sender;

        /// <summary>
        /// 上报配置和结果
        /// </summary>
        private PullSocket responseSocket;

        //接收全局信息，可能是tool 、gm等后台，广播给worker
        private Proxy GlobalServerProxy { get; set; }

        public NetMQQueue<NetMQMessage> PushQueue = new NetMQQueue<NetMQMessage>();

        private NetMQPoller Poller;
        private bool IsSetuped { get; set; }
        public virtual void Setup(INetManager netManager)
        {
            if (string.IsNullOrEmpty(PushsocketString))
                throw new ArgumentNullException(nameof(PushsocketString));
            if (string.IsNullOrEmpty(PullSocketString))
                throw new ArgumentNullException(nameof(PullSocketString));
            if (netManager == null)
                throw new ArgumentNullException(nameof(netManager));

            if (!string.IsNullOrEmpty(XPubSocketString) && !string.IsNullOrEmpty(XSubSocketString))
            {
                var PublisherSocket = new XPublisherSocket(XPubSocketString);

                var SubscriberSocket = new XSubscriberSocket(XSubSocketString);

                GlobalServerProxy = new Proxy(SubscriberSocket, PublisherSocket);
            }

            sender = new PushSocket(PushsocketString);
            responseSocket = new PullSocket(PullSocketString);

            Poller = new NetMQPoller()
            {
                sender,
                responseSocket,
                PushQueue
            };

            NetSend = netManager;

            PushQueue.ReceiveReady += (o, args) =>
            {
                var msg = PushQueue.Dequeue();
                sender.SendMultipartMessage(msg);
            };

            responseSocket.ReceiveReady += ProcessResponse;

            IsSetuped = true;
        }
        //服务器向客户端下发数据
        protected virtual void ProcessResponse(object o, NetMQSocketEventArgs e)
        {
            //TODO https://netmq.readthedocs.io/en/latest/poller/   #Performance

            var msg1 = responseSocket.ReceiveMultipartMessage();//TryReceiveMultipartMessage();
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    var msg = (NetMQMessage)state;

                    //msg[0].Buffer //版本号
                    var action = msg[1].Buffer[0];
                    var sessionid = msg[2].ConvertToString();

                    var clientkey = msg[3].Buffer;
                    var serverkey = msg[4].Buffer;
                    var crypto = msg[5].Buffer[0];

                    var lastNo = msg[6].ConvertToInt32();

                    var header = msg[7].Buffer;
                    var content = msg[8].Buffer;


                    var response = new PirateXResponsePackage()
                    {
                        HeaderBytes = header,
                        ContentBytes = content
                    };

#if DEBUG
                    var r = new PirateXResponseInfo(response);
                    r.Headers.Add("_tout_", $"{DateTime.UtcNow.Ticks}");
                    response.HeaderBytes = r.GetHeaderBytes();

#endif

                    var protocolPackage = NetSend.GetProtocolPackage(sessionid);

                    //将消息下发到客户端
                    if (protocolPackage == null)
                        return;
                    if (protocolPackage.PackKeys == null)
                        protocolPackage.PackKeys = serverkey;

                    if (protocolPackage.UnPackKeys == null)
                        protocolPackage.UnPackKeys = clientkey;

                    if (lastNo >= 0)
                        protocolPackage.LastNo = lastNo;

                    var bytes = protocolPackage.PackPacketToBytes(response);
                    NetSend.Send(sessionid, bytes);

                    protocolPackage.CryptoByte = crypto;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }, msg1);
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

            var request = protocolPackage.UnPackToPacket(body);

#if DEBUG
            var r = new PirateXRequestInfo(request);
            r.Headers.Add("_tin_", $"{DateTime.UtcNow.Ticks}");
            request = r.ToRequestPackage();
#endif

            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append(new byte[] { (byte)Action.Req });//动作
            msg.Append(protocolPackage.SessionID);//sessionid
            msg.Append(protocolPackage.PackKeys);//客户端密钥
            msg.Append(protocolPackage.UnPackKeys);//服务端密钥
            msg.Append(request.HeaderBytes);//信息头
            msg.Append(request.ContentBytes);//信息体
            msg.Append((protocolPackage.RemoteEndPoint as IPEndPoint).Address.ToString());
            msg.Append(new byte[] { protocolPackage.CryptoByte });
            msg.Append(protocolPackage.LastNo);
            //加入队列
            PushQueue.Enqueue(msg);
        }


        public virtual void Ping()
        {
            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append(new byte[] { (byte)Action.Ping });//动作
            //加入队列
            PushQueue.Enqueue(msg);
        }

        public virtual void OnSessionClosed(ProtocolPackage protocolPackage)
        {
            if (protocolPackage == null)
                return;

            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append(new byte[] { (byte)Action.Closed });//动作
            msg.Append(protocolPackage.SessionID);//sessionid
            msg.Append(protocolPackage.PackKeys);//客户端密钥
            msg.Append(protocolPackage.UnPackKeys);//服务端密钥
            msg.Append(new byte[] { });//信息头
            msg.Append(new byte[] { });//信息体
            msg.Append((protocolPackage.RemoteEndPoint as IPEndPoint).Address.ToString());
            msg.Append(new byte[] { protocolPackage.CryptoByte });
            msg.Append(protocolPackage.LastNo);
            
            PushQueue.Enqueue(msg);
        }

        public virtual void Start()
        {
            if (!IsSetuped)
                throw new ApplicationException("Please Setup firset!");

            Poller.RunAsync();
            GlobalServerProxy?.Start();
        }

        public virtual void Stop()
        {
            GlobalServerProxy?.Stop();

            Poller?.Stop();
            Poller?.Dispose();
            Poller = null;
        }
    }


    enum Action:byte
    {
        Ping = 0 ,
        Req  = 1 , 
        Closed = 2 ,
    }
}
