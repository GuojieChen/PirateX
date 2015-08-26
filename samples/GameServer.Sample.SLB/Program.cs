using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Logging;
using GameServer.SLB;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;

namespace GameServer.Sample.SLB
{
    class Program
    {
        static void Main(string[] args)
        {

            var server = new SLBServer();

            server.Setup(new RootConfig()
            {
                LogFactory = "NLogLogFactory",
            },new ServerConfig()
            {
                Port = 3001,
                MaxConnectionNumber = 100
            });

            server.Start();

            var a = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("{0,20}\t{1,-4}", server.Name, server.State);
            System.Console.ForegroundColor = a;

            Console.Read();
            Console.Read();
        }
    }
}
