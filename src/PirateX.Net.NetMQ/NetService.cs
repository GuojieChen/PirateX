﻿using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core;
using PirateX.Core.Net;
using PirateX.Core.Utils;
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

        public NetMQQueue<byte[]> PushQueue = new NetMQQueue<byte[]>();

        private NetMQPoller Poller;

        private NetMQPoller ResponsePoller; 

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
                PushQueue
            };

            ResponsePoller = new NetMQPoller()
            {
                responseSocket,
            };

            NetSend = netManager;

            PushQueue.ReceiveReady += (o, args) =>
            {
                sender.SendFrame(args.Queue.Dequeue());
            };

            responseSocket.ReceiveReady += ProcessResponse;

            IsSetuped = true;
        }



        //服务器向客户端下发数据
        protected virtual void ProcessResponse(object o, NetMQSocketEventArgs e)
        {
            //TODO https://netmq.readthedocs.io/en/latest/poller/   #Performance

            var msg = responseSocket.ReceiveFrameBytes();//TryReceiveMultipartMessage();
           /* ThreadPool.QueueUserWorkItem(state =>
            {*/
                try
                {
                    //var msg = msg1;//(byte[])state;

                    var dout = msg.FromProtobuf<Out>();

                    //msg[0].Buffer //版本号
                    //var action = msg[1].Buffer[0];
                    //var sessionid = msg[2].ConvertToString();
                    //var lastNo = msg[3].ConvertToInt32();
                    //var header = msg[4].Buffer;
                    //var content = msg[5].Buffer;
                    //var rid = msg[6].ConvertToInt32();


                    var response = new PirateXResponsePackage()
                    {
                        HeaderBytes = dout.HeaderBytes,
                        ContentBytes = dout.BodyBytes
                    };


                    IProtocolPackage protocolPackage = null;

                    if (Equals(dout.Action, Action.Seed))
                    {
                        protocolPackage = NetSend.GetProtocolPackage(dout.SessionId);
                        protocolPackage.Rid = dout.Id;

                        NetSend.Attach(protocolPackage);
                    }
                    else
                        protocolPackage = NetSend.GetProtocolPackage(dout.Id);


#if PERFORM
                    var r = new PirateXResponseInfo(response);
                    r.Headers.Add("_tout_", $"{DateTime.UtcNow.Ticks}");
                    response.HeaderBytes = r.GetHeaderBytes();


                    new ProfilerLog()
                    {
                        Token = r.Headers["token"],
                        Ip = protocolPackage.RemoteEndPoint.ToString(),
                        Tout = new Ticks()
                        {
                            Start = Convert.ToInt64(r.Headers["_itout_"]),
                            End = Convert.ToInt64(r.Headers["_tout_"])
                        }
                    }.Log();

#endif


                    if (dout.LastNo >= 0)
                        protocolPackage.LastNo = dout.LastNo;

                    var bytes = protocolPackage.PackPacketToBytes(response);
                    protocolPackage.Send(bytes);

                    if (Equals(dout.Action, Action.Seed))
                    {
                        var clientkey = dout.ClientKeys;
                        var serverkey = dout.ServerKeys;
                        var crypto = dout.Crypto;

                        //种子交换  记录种子信息，后续收发数据用得到
                        protocolPackage.PackKeys = serverkey;
                        protocolPackage.UnPackKeys = clientkey;
                        protocolPackage.CryptoByte = crypto;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
//            }, msg1);
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
        /// 收到客户端请求，交由Actor进行处理
        /// </summary>
        /// <param name="protocolPackage"></param>
        /// <param name="body"></param>
        public virtual void ProcessRequest(IProtocolPackage protocolPackage, byte[] body)
        {
            if (protocolPackage == null)
                return;

            if (protocolPackage.CryptoByte > 0)
            {
                var last = NetSend.GetProtocolPackage(protocolPackage.Rid);
                if (!Equals(last.Id, protocolPackage.Id))
                {
                    protocolPackage.Close();
                    return;
                }
            }

            var request = protocolPackage.UnPackToPacket(body);

#if PERFORM || DEBUG
            var r = new PirateXRequestInfo(request);
            r.Headers.Add("_tin_", $"{DateTime.UtcNow.Ticks}");
            request = r.ToRequestPackage();
#endif

            //加入队列
            PushQueue.Enqueue(new In()
            {
                Version = 1,
                Action = Action.Req,
                HeaderBytes = request.HeaderBytes,
                QueryBytes = request.ContentBytes,
                Ip = (protocolPackage.RemoteEndPoint as IPEndPoint).Address.ToString(),
                LastNo = protocolPackage.LastNo,
                SessionId = protocolPackage.Id,
            }.ToProtobuf());
        }


        public virtual void Ping()
        {
            PushQueue.Enqueue(new In()
            {
                Version = 1,
                Action = Action.Ping,
            }.ToProtobuf());
        }

        public virtual void OnSessionClosed(IProtocolPackage protocolPackage)
        {
            if (protocolPackage == null)
                return;

            //加入队列
            PushQueue.Enqueue(new In()
            {
                Version = 1,
                Action = Action.Closed,
                Ip = (protocolPackage.RemoteEndPoint as IPEndPoint).Address.ToString(),
                LastNo = protocolPackage.LastNo,
                SessionId = protocolPackage.Id,
            }.ToProtobuf());
        }

        public virtual void Start()
        {
            if (!IsSetuped)
                throw new ApplicationException("Please Setup firset!");

            Poller.RunAsync();
            ResponsePoller.RunAsync();
            GlobalServerProxy?.Start();
        }

        public virtual void Stop()
        {
            GlobalServerProxy?.Stop();

            Poller?.Stop();
            Poller?.Dispose();
            Poller = null;

            ResponsePoller?.Stop();
            ResponsePoller?.Dispose();
            ResponsePoller = null;

        }
    }


    
}
