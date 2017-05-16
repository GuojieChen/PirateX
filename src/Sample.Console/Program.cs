using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Console.Cmd;
using PirateX.Core.i18n;
using PirateX.Core.Utils;
using PirateX.Net;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using ProtoBuf;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using Topshelf;

namespace GameServer.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProtoBuf.Serializer.PrepareSerializer<Token>();
            var host = HostFactory.New(c =>
            {
                c.UseNLog() ;
                
                c.Service<AllServices>(s =>
                {
                    s.ConstructUsing(name => new AllServices());
                    
                    s.WhenStarted(t => t.Start());

                    s.WhenStopped(t => t.Stop());
                });
            });

            host.Run();
        }
    }
}