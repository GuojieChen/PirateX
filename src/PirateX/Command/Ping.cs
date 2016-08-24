using PirateX.Core.Utils;
using PirateX.Protocol;
using SuperSocket.SocketBase;

namespace PirateX.Command
{
    public class Ping<TSession>: GameCommand<TSession, NoneRequest, PingResponse>
        where TSession : GameSession<TSession>, IAppSession<TSession, IGameRequestInfo>, new()
    {
        public override string Name => "Ping"; 

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
