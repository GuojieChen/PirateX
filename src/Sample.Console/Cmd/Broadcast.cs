using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Command;
using PirateX.Core.Broadcas;
using PirateX.Protocol;

namespace GameServer.Console.Cmd
{
    public class Broadcast : GameCommand<DemoSession, BroadcastRequest, NoneResponse>
    {
        protected override NoneResponse ExecuteResponseCommand(DemoSession session, BroadcastRequest data)
        {
            var b = session.Reslover.Resolve<IMessageBroadcast>();
            b.Send(data,1,2,3);

            return null; 
        }
    }

    public class BroadcastRequest
    {
        public string Message { get; set; }
    }
}
