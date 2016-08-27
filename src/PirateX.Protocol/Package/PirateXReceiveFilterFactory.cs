using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol.Package
{
    public class PirateXReceiveFilterFactory :IReceiveFilterFactory<IPirateXRequestInfo>
    {
        public IReceiveFilter<IPirateXRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new PirateXReceiveFilter((IGameSessionBase)appSession);
        }
    }
}
