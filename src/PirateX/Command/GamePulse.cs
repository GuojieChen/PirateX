using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol;
using SuperSocket.SocketBase;

namespace PirateX.Command
{
    public class GamePulse<TSession> : GameCommand<TSession,NoneRequest,NoneResponse> where TSession : IGameSession, IAppSession<TSession, IGameRequestInfo>, new()
    {
        protected override NoneResponse ExecuteResponseCommand(TSession session, NoneRequest data)
        {



            return null; 
        }
    }
}
