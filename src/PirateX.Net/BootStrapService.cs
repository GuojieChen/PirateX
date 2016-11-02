using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace PirateX.Net
{
    public class BootStrapService
    {
        private IBootstrap bootstrap;

        public void Start()
        {
            bootstrap = BootstrapFactory.CreateBootstrap();
            bootstrap.Start();
            bootstrap.Initialize();
        }

        public void Stop()
        {
            bootstrap?.Stop();
        }
    }
}
