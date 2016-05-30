using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol.V1
{
    public class JsonReceiveFilterFactory : IReceiveFilterFactory<IGameRequestInfo>
    {
        public IReceiveFilter<IGameRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new JsonReceiveFilter((IGameSessionBase)appSession);
        }
    }
}
