using System.Net;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol.Package
{
    public class JsonReceiveFilterFactory : IReceiveFilterFactory<IGameRequestInfo>
    {
        public IReceiveFilter<IGameRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new JsonReceiveFilter((IGameSessionBase)appSession);
        }
    }
}
