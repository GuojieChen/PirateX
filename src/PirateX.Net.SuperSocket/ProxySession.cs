using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol.Package;
using SuperSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net
{
    public class ProxySession:AppSession<ProxySession,BinaryRequestInfo>
    {
        public ProtocolPackage ProtocolPackage { get; private set; }

        public ProxySession()
        {
            this.ProtocolPackage = new ProtocolPackage()
            {
                SessionID = this.SessionID
            };
        }
    }
}
