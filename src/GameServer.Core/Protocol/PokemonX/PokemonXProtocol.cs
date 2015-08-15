using System.Net;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Protocol.PokemonX
{
    public class PokemonXProtocol : IReceiveFilterFactory<IPokemonXRequestInfo>
    {
        public IReceiveFilter<IPokemonXRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new PokemonXReceiveFilter((IGameSession)appSession);
        }
    }
}
