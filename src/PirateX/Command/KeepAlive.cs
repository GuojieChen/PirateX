using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;

namespace PirateX.Command
{
    public class KeepAlive<TSession> : GameCommand<TSession,NoneRequest,NoneResponse> 
        where TSession : IPirateXSession, IAppSession<TSession, IPirateXRequestInfo>, new()
    {
        public override string Name => "KeepAlive";

        protected override NoneResponse ExecuteResponseCommand(TSession session, NoneRequest data)
        {



            return null; 
        }
    }
}
