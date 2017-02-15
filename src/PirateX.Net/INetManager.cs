using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol.Package;

namespace PirateX.Net
{
    public interface INetManager
    {
        string PushsocketString { get; set; }

        string PullSocketString { get; set; }

        /// <summary>
        /// https://netmq.readthedocs.io/en/latest/xpub-xsub/
        /// </summary>
        string XPubSocketString { get; set; }
        /// <summary>
        /// https://netmq.readthedocs.io/en/latest/xpub-xsub/
        /// </summary>
        string XSubSocketString { get; set; }

        ProtocolPackage GetProtocolPackage(string sessionid);

        void Send(string sessionid,byte[] datas);

    }
}
