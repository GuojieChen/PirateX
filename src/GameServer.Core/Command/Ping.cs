using GameServer.Core.PkProtocol;
using SuperSocket.SocketBase.Command;

namespace GameServer.Core.Command
{
    public class Ping<TSession>: CommandBase<TSession,ISocketRequestInfo> where TSession : PSession<TSession>, new()
    {
        public override void ExecuteCommand(TSession session, ISocketRequestInfo requestInfo)
        {
            //返回数据
            session.SendMessage(new
            {
                C = Name,
                O = session.CurrentO,
                D = new { T = Utils.GetTimestamp() }
            });
        }
    }
}
