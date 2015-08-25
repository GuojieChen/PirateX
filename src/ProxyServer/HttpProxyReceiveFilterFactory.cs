using System.Net;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PokemonX.ProxyServer
{
    class HttpProxyReceiveFilterFactory : IReceiveFilterFactory<BinaryRequestInfo>
    {
        public IReceiveFilter<BinaryRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new HttpProxyReceiveFilter((ProxySession)appSession);
        }
    }
}
