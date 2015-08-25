using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonX.SuperSocket;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PokemonX.ProxyServer
{
    public class PokemonServer : PServer<PokemonSession>
    {
        public PokemonServer():base(new PokemonProxyReceiveFilterFactory())
        {
        }

    }
}
