using PirateX.Protocol.Package;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public class ProxySession:AppSession<ProxySession,BinaryRequestInfo>
    {
        public ProtocolPackage ProtocolPackage { get; private set; }


        public new GameAppServer AppServer => (GameAppServer) base.AppServer;


        public ProxySession()
        {
            this.ProtocolPackage = new ProtocolPackage()
            {
                SessionID = this.SessionID
            };
        }
    }
}
