using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Console.Cmd;
using PirateX.Core;
using PirateX.Net;
using PirateX.Net.NetMQ;
using PirateX.Protocol;
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
            var a = new A();
            var b = new B();
            var t = new T1();

            System.Console.WriteLine(A.V);
            t.V = 10;
            b.Do(t);
            System.Console.WriteLine(t.V++);
            System.Console.Read();

            //ProtoBuf.Serializer.PrepareSerializer<Token>();
            //ProtoBuf.Serializer.PrepareSerializer<In>();
            //ProtoBuf.Serializer.PrepareSerializer<Out>();
            //System.Console.WriteLine(ProtoBuf.Serializer.GetProto<ProtoSyncResponse>());
            //var host = HostFactory.New(c =>
            //{
            //    c.UseNLog();

            //    c.Service<AllServices>(s =>
            //    {
            //        s.ConstructUsing(name => new AllServices()); 

            //        s.WhenStarted(t => t.Start());

            //        s.WhenStopped(t => t.Stop());
            //    });
            //});

            //host.Run();
        }
    }
}