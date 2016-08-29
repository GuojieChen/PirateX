using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.UnitTest
{
    public class TestServer: PirateXServer<TestSession,OnlineRole>
    {
        public TestServer()
             : base(new TestContainer(), new PirateXReceiveFilterFactory())
        {
        }

        public override Assembly ConfigAssembly()
        {
            return typeof (TestServer).GetType().Assembly;
        }

        public override void IocConfig(ContainerBuilder builder)
        {
            builder.Register(c => new ProtoResponseConvert()).As<IResponseConvert>().SingleInstance();
        }
    }
}
