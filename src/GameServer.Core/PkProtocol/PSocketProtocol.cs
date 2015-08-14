using System.Net;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.PkProtocol
{
    public class PSocketProtocol : IReceiveFilterFactory<ISocketRequestInfo>
    {
        public IReceiveFilter<ISocketRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new PSocketHeaderReceiveFilter((IGameSession)appSession);
        }
    }
}
