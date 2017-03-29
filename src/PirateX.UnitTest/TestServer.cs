using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.Core.Container;
using PirateX.Net.Actor;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.UnitTest
{
    public class TestServer: ActorService<OnlineRole>
    {
        public TestServer(Net.ActorConfig config, IServerContainer serverContainer) : base(config, serverContainer)
        {
        }

        public override Assembly ConfigAssembly()
        {
            return typeof (TestServer).GetType().Assembly;
        }

        public override void IocConfig(ContainerBuilder builder)
        {
            //builder.Register(c => new ProtoResponseConvert()).As<IResponseConvert>().SingleInstance();
        }
    }
}
