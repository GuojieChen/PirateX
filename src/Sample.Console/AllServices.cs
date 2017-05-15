using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Actor;
using PirateX.Net;
using PirateX.Net.NetMQ;
using PirateX.Net.SuperSocket;
using ServiceStack.Common.Extensions;
using ServiceStack.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine.Configuration;
using SuperSocket.SocketBase.Config;

namespace GameServer.Console
{
    public class AllServices
    {
        private GameAppServer HostServer;

        private ActorNetService[] WorkerServer;

        public AllServices()
        {
            HostServer = new GameAppServer(new NetService()
            {
                PullSocketString = "@tcp://localhost:5001",
                PushsocketString = "@tcp://localhost:5002",
                XPubSocketString = "",
                XSubSocketString = ""
            });
            var b = HostServer.Setup(new ServerConfig()
            {
                Port = 4012,
            });

            WorkerServer = new ActorNetService[]
            {
                new ActorNetService(new WorkerService(),new ActorConfig
                {
                    PushsocketString = ">tcp://localhost:5001",
                    PullSocketString = ">tcp://localhost:5002",
                }),
                //new ActorNetService(new WorkerService(),new ActorConfig()
                //{
                //    PushsocketString = ">tcp://localhost:5001",
                //    PullSocketString = ">tcp://localhost:5002",
                //})
            };
        }


        public void Start()
        {
            System.Console.WriteLine("start!");
            HostServer.Start();

            WorkerServer.ForEach(item=>item.Start());
        }

        public void Stop()
        {
            HostServer.Stop();
            WorkerServer.ForEach(item => item.Stop());
        }
    }
}
