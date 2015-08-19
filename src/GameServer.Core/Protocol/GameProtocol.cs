﻿using System.Net;
using GameServer.Core.Protocol.V1;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Protocol
{
    public class PokemonXProtocol : IReceiveFilterFactory<IRequestInfo>
    {
        public IReceiveFilter<IRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new JsonReceiveFilter((IGameSession)appSession);
        }
    }
}
