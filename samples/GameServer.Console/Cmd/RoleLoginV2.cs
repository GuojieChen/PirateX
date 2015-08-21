using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core;
using GameServer.Core.Protocol;

namespace GameServer.Console.Cmd
{
    public class RoleLoginV2:GameCommand<DemoSession,RoleLoginV2Request, RoleLoginV2Request>
    {
        protected override RoleLoginV2Request ExecuteResponseCommand(DemoSession session, RoleLoginV2Request data)
        {
            System.Console.WriteLine("OK");

            return data;
        }
    }

    public class RoleLoginV2Request
    {
        public long Rid { get; set; }

        public int ServerId { get; set; }
    }
}
