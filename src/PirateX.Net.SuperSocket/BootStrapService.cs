using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace PirateX.Net.SuperSocket
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
