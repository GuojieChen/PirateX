using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.Core.Actor;
using PirateX.Core.Container;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.UnitTest
{
    public class TestServer: ActorService<TestServer,OnlineRole>
    {
        public TestServer(ActorConfig config, IServerContainer serverContainer) : base( serverContainer)
        {
        }

        protected override OnlineRole CreateOnlineRole(ActorContext context, IToken token)
        {
            return new OnlineRole();
        }

        public override void IocConfig(ContainerBuilder builder)
        {
            //builder.Register(c => new ProtoResponseConvert()).As<IResponseConvert>().SingleInstance();
        }
    }
}
