using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Protocol;
using PirateX.Sync.ProtoSync;
using SuperSocket.SocketBase;

namespace PirateX.Command.System
{
    public class SysinfoAction<TSession> : GameCommandBase<TSession, NoneRequest>
        where TSession : GameSession<TSession>, IAppSession<TSession, IGameRequestInfo>, new()
    {
        public override string Name => "_sysinfo"; 

        protected override void ExecuteGameCommand(TSession session, NoneRequest data)
        {
            var protoservice = ((IGameServer) session.AppServer).ServerContainer.ServerIoc.Resolve<IProtoService>();

            session.SendMessage(new ProtocolMessage()
            {
                C = this.Name,
                D =new 
                {
                    Version = "1.0.0",
                    protohash = protoservice.GetProtosHash(),//
                    datahash = "",
                }
            });
        }
    }
}
