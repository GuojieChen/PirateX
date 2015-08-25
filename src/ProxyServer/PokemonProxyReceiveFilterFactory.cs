using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using PokemonX.SuperSocket.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using ISocketSession = PokemonX.SuperSocket.ISocketSession;

namespace PokemonX.ProxyServer
{
    class PokemonProxyReceiveFilterFactory : IReceiveFilterFactory<ISocketRequestInfo>
    {
        public IReceiveFilter<ISocketRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new PokemonSocketHeaderReceiveFilter((PokemonSession)appSession);
        }
    }
}
