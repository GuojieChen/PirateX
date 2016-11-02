using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using PirateX.Protocol.Package;
using SuperSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net
{
    public class ProxySession:AppSession<ProxySession,BinaryRequestInfo>
    {
        public IProtocolPackage ProtocolPackage { get; set; }

        public ProxySession()
        {
            ProtocolPackage = new ProtocolPackage();
        }
    }
}
