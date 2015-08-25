using System.Net;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Proxy.V1
{

    public class GameProxyFilterFactory : IReceiveFilterFactory<BinaryRequestInfo>
    {
        public IReceiveFilter<BinaryRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new GameProxyRequestReceiveFilter((GameProxySession)appSession);
        }
    }
}
