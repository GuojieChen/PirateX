using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Actor;

namespace GameServer.Console.Cmd
{
    public class Test:RepAction<EmptyBodyResponse>
    {
        public override EmptyBodyResponse Play()
        {
            return null;
        }
    }
}
