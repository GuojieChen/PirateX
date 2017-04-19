using PirateX.Core.Utils;

namespace PirateX.Core.Actor
{
    public class Ping : RepAction<NewSeedResponse>
    {
        public override NewSeedResponse Play()
        {
            return new NewSeedResponse() { Seed = TimeUtil.GetTimestampAsSecond() };
        }
    }
}
