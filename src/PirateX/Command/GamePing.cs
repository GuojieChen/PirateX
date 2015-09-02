﻿using PirateX.Protocol;
using PirateX.Utils;
using SuperSocket.SocketBase;

namespace PirateX.Command
{
    public class GamePing<TSession>: GameCommand<TSession, NoneRequest, PingResponse>
        //CommandBase<TSession,GameRequestInfoV1> 
        where TSession : PSession<TSession>, IAppSession<TSession, IGameRequestInfo>, new()
    {
        protected override PingResponse ExecuteResponseCommand(TSession session, NoneRequest data)
        {
            return new PingResponse()
            {
                T = TimeUtil.GetTimestamp()
            };
        }
    }

    public class PingResponse
    {
        public long T { get; set; }
    }
}