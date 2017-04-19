using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Actor;
using PirateX.Core.Broadcas;
using PirateX.Protocol;

namespace GameServer.Console.Cmd
{
    public class Broadcast : RepAction
    {
        public override void Execute()
        {

            var b = Reslover.Resolve<IMessageBroadcast>();
            b.Send(Context.Request, 1, 2, 3);
        }
    }

}
