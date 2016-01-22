using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using PirateX.Core.i18n;
using PirateX.Core.Utils;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace GameServer.Console
{
    internal class Program
    {
        private static void Main(string[] args)
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
            foreach (var appServer in bootstrap.AppServers)
            {
                var a = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("{0,20}\t{1,-4}", appServer.Name, appServer.State);
                System.Console.ForegroundColor = a;
            }

            System.Console.WriteLine("Press key 'q' to stop it!");

            while (System.Console.ReadKey().KeyChar != 'q')
            {
                System.Console.WriteLine();
            }

            System.Console.WriteLine();

            //GameStop the appServer
            bootstrap.Stop();

            System.Console.WriteLine("The server was stopped!");

            System.Console.Read();
        }
    }
}