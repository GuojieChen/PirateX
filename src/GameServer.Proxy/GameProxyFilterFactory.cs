using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Proxy
{

    public class GameProxyFilterFactory : IReceiveFilterFactory<BinaryRequestInfo>
    {
        public IReceiveFilter<BinaryRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new GameProxyRequestReceiveFilter((GameProxySession)appSession);
        }
    }
}
