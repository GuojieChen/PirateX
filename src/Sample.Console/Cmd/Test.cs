﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;

namespace GameServer.Console.Cmd
{
    public class Test:RepAction<EmptyBodyResponse>
    {
        public override EmptyBodyResponse Play()
        {

            var value = base.ServerReslover.ResolveKeyed<IDbConnection>("role");
            
            

            return null;
        }
    }
}
