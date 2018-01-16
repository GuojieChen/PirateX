using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Utils;
using PirateX.Net.NetMQ;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public class PushCmd : CommandBase<ProxySession,BinaryRequestInfo>
    {
        public override void ExecuteCommand(ProxySession session, BinaryRequestInfo requestInfo)
        {
            var msg = session.AppServer.NetService.ProcessRequest(session, requestInfo.Body);

            var dout = msg.FromProtobuf<Out>();

            var response = new PirateXResponsePackage()
            {
                HeaderBytes = dout.HeaderBytes,
                ContentBytes = dout.BodyBytes
            };
        }
    }
}
