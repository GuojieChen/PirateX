using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Net;
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

        private WorkerService[] WorkerServer;

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
                Ip = "192.168.1.34"
            });

            WorkerServer = new WorkerService[]
            {
                new WorkerService(new ActorConfig()
                {
                    PushsocketString = ">tcp://localhost:5001",
                    PullSocketString = ">tcp://localhost:5002",
                }),
                new WorkerService(new ActorConfig()
                {
                    PushsocketString = ">tcp://localhost:5001",
                    PullSocketString = ">tcp://localhost:5002",
                })
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
