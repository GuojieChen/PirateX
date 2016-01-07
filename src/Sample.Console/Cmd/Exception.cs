using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.GException;
using PirateX.Protocol;

namespace GameServer.Console.Cmd
{
    public class Exception : GameCommand<DemoSession, RoleInfoRequest, RoleInfoResponse>
    {
        protected override RoleInfoResponse ExecuteResponseCommand(DemoSession session, RoleInfoRequest data)
        {
            throw new FreezeException();
        }
    }
}
