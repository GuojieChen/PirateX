using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol
{
    public class PirateRequestInfo2: PirateXRequestInfo, IPirateXRequestInfo
    {

        public PirateRequestInfo2(byte[] headerBytes, byte[] contentBytes) : base(headerBytes, contentBytes)
        {
        }

        public PirateRequestInfo2(IPirateXRequestPackage requestPackage) : base(requestPackage)
        {

        }
    }
}
