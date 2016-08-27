using PirateX.Core.Utils;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;

namespace PirateX.Command
{
    public class Ping<TSession>: GameCommand<TSession, NoneRequest, PingResponse>
        where TSession : PirateXSession<TSession>, IAppSession<TSession, IPirateXRequestInfo>, new()
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
