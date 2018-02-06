using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using GameServer.Console.SampleDomain;
using PirateX.Core;

namespace GameServer.Console.Cmd
{
    public class Test:RepAction<EmptyBodyResponse>
    {
        public override EmptyBodyResponse Play()
        {

            //var value = base.ServerReslover.ResolveKeyed<IDbConnection>("role");


            Thread.Sleep(100);


            base.MessageSender.PushMessage<Role>(2457,new Role(){Id =1,Name = "GuojieChen"});
            

            return null;
        }
    }
}
