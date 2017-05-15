using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public class PushCmd : CommandBase<ProxySession,BinaryRequestInfo>
    {
        public override void ExecuteCommand(ProxySession session, BinaryRequestInfo requestInfo)
        {
            session.ProtocolPackage.SessionID = session.SessionID;
            session.ProtocolPackage.RemoteEndPoint = session.RemoteEndPoint;

            session.AppServer.NetService.ProcessRequest(session.ProtocolPackage, requestInfo.Body);
        }
    }
}
