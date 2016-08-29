using System.Net;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol
{
    public class PirateXReceiveFilterFactory :IReceiveFilterFactory<IPirateXRequestInfo>
    {
        public IReceiveFilter<IPirateXRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new PirateXReceiveFilter((IPirateXSession)appSession);
        }
    }
}
