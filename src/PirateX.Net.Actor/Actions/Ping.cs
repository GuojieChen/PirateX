using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Utils;

namespace PirateX.Net.Actor.Actions
{
    public class Ping : RepAction<NewSeedResponse>
    {
        public override NewSeedResponse Play()
        {
            return new NewSeedResponse() {Seed = TimeUtil.GetTimestampAsSecond() };
        }
    }
}
