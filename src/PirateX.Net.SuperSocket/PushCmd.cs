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
            session.AppServer.NetService.ProcessRequest(session, requestInfo.Body);
        }
    }
}
