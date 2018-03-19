using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using GameServer.Console.SampleDomain;
using PirateX.Core;
using PirateX.Protocol;

namespace GameServer.Console.Cmd
{
    public class Broadcast : RepAction
    {
        public override void Execute()
        {

            //var b = Resolver.Resolve<IMessageBroadcast>();
            //b.Send(Context.Request, 1, 2, 3);

            //base.MessageSender.PushMessage<Role>(Context.Token.Rid,new Role(){Name = "GuojieChen"});
        }
    }

}
