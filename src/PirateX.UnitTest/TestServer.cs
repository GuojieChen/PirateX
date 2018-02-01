using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.Protocol;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.UnitTest
{
    public class TestServer: ActorService<TestServer>
    {
        public TestServer(ActorConfig config, IDistrictContainer serverContainer) : base( serverContainer)
        {
        }
        
    }
}
