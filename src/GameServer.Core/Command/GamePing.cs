using System;
using GameServer.Core.Protocol;
using GameServer.Core.Protocol.V1;
using GameServer.Core.Utils;
using SuperSocket.SocketBase;

namespace GameServer.Core.Command
{
    public class GamePing<TSession>: GameCommand<TSession, PingRequest, PingResponse>
        //CommandBase<TSession,GameRequestInfoV1> 
        where TSession : PSession<TSession, Enum>, IAppSession<TSession, JsonRequestInfo>, new()
    {
        protected override PingResponse ExecuteResponseCommand(TSession session, PingRequest data)
        {
            return new PingResponse()
            {
                T = TimeUtil.GetTimestamp()
            };
        }
    }
}
