﻿using System.Net;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PokemonX.ProxyServer
{
    class SocksProxyReceiveFilterFactory : IReceiveFilterFactory<BinaryRequestInfo>
    {
        public IReceiveFilter<BinaryRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new SocksSwitchReceiveFilter((ProxySession)appSession);
        }
    }
}
