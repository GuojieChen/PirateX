using System;
using GameServer.Core.Utils;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;

namespace GameServer.Core.Protocol.PokemonX
{
    public class Ping<TSession>: CommandBase<TSession,IPokemonXRequestInfo> 
        where TSession : PSession<TSession,Enum>, IAppSession<TSession, IPokemonXRequestInfo>, new()
    {
        public override void ExecuteCommand(TSession session, IPokemonXRequestInfo requestInfo)
        {
            //返回数据
            session.SendMessage(new
            {
                C = Name,
                O = session.CurrentO,
                D = new { T = TimeUtil.GetTimestamp() }
            });
        }
    }
}
