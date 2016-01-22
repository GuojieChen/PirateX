using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ProtoBuf;

namespace PirateX.Core.Service
{
    /// <summary>
    /// proto协议描述同步服务
    /// </summary>
    public class SyncProtobufService
    {
        public ILifetimeScope Resolver { get; set; }

        public void InitProto()
        {
            /*
            var proto = Serializer.GetProto<>();
            System.Console.WriteLine(proto);

            System.Console.Read();
            */
        }


    }
}
