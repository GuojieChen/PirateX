using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX;
using PirateX.Protocol;

namespace GameServer.Console.Cmd
{
    public class RoleInfo: GameCommand<DemoSession,RoleInfoRequest,RoleInfoResponse>
    {
        protected override RoleInfoResponse ExecuteResponseCommand(DemoSession session, RoleInfoRequest data)
        {
            return new RoleInfoResponse();
        }
    }

    public class RoleInfoRequest
    {
        
    }

    public class RoleInfoResponse
    {
        
    }
}
