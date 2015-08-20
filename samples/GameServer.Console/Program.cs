﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Logging;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;

namespace GameServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrap = BootstrapFactory.CreateBootstrap();

            if (!bootstrap.Initialize())
            {
                System.Console.WriteLine("Failed to initialize!");
                System.Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();

            System.Console.WriteLine("Start result: {0}!", result);

            if (result == StartResult.Failed)
            {
                System.Console.WriteLine("Failed to start!");
                System.Console.WriteLine(result);
                System.Console.ReadKey();
                return;
            }
            else
            {
                foreach (var appServer in bootstrap.AppServers)
                {
                    System.Console.WriteLine("{0,20}\t{1,-4}", appServer.Name, appServer.State);
                }
            }

            System.Console.WriteLine("Press key 'q' to stop it!");

            while (System.Console.ReadKey().KeyChar != 'q')
            {
                System.Console.WriteLine();
                continue;
            }

            System.Console.WriteLine();

            //GameStop the appServer
            bootstrap.Stop();

            System.Console.WriteLine("The server was stopped!");

            System.Console.Read();
        }
    }
}
