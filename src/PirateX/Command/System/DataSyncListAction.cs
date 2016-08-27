using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol;
using SuperSocket.SocketBase;

namespace PirateX.Command.System
{
    public class DataSyncListAction : GameCommand<PirateXSession,NoneRequest,NoneResponse>
    {
        public override string Name => "_synclist"; 

        protected override NoneResponse ExecuteResponseCommand(PirateXSession session, NoneRequest data)
        {

            return null; 
        }
    }
}
