using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol.Package;

namespace PirateX.Net
{
    public interface INetSend
    {
        ProtocolPackage GetProtocolPackage(string sessionid);

        void Send(string sessionid,byte[] datas);

    }
}
