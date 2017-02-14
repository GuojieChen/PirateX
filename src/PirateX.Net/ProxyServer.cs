using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net
{
    public class ProxyServer:AppServer<ProxySession,BinaryRequestInfo>
    {

        /// <summary>
        /// 下发任务
        /// </summary>
        private static PushSocket sender = new PushSocket("@tcp://*:5557");
        /// <summary>
        /// 上报配置和结果
        /// </summary>
        private static PullSocket responseSocket = new PullSocket("@tcp://*:5558");

        public static NetMQQueue<NetMQMessage> PushQueue = new NetMQQueue<NetMQMessage>(); 

        private NetMQPoller Poller = new NetMQPoller()
        {
            sender,
            responseSocket,
            PushQueue
        };




        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            PushQueue.ReceiveReady += (o, args) =>
            {
                var msg = PushQueue.Dequeue();
                sender.SendMultipartMessage(msg);
            };


            responseSocket.ReceiveReady += ProcessRequest;

            return base.Setup(rootConfig, config);
        }

        private void ProcessRequest(object o, NetMQSocketEventArgs e)
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

            //转发
            this.AsyncRun(() =>
            {
                //将消息下发到客户端
                var session = GetSessionByID(sessionid);
                if (session != null)
                {
                    if(session.ProtocolPackage.ClientKeys == null)
                        session.ProtocolPackage.ClientKeys = clientkey;
                    if(session.ProtocolPackage.ServerKeys == null)
                        session.ProtocolPackage.ServerKeys = serverkey;

                    var bytes = session.ProtocolPackage.PackResponsePackageToBytes(response);
                    session.Send(bytes, 0, bytes.Length);
                }
            });
        }

        protected override void OnStarted()
        {
            base.OnStarted();

            Poller.RunAsync();
        }
    }
}
