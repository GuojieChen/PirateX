using System.Net;
using PirateX.Protocol.V1;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol
{
    public class PokemonXProtocol : IReceiveFilterFactory<IGameRequestInfo>
    {
        public IReceiveFilter<IGameRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new JsonReceiveFilter((IGameSession)appSession);
        }
    }
}
