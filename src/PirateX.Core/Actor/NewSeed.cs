using System;
using System.Collections;
using PirateX.Core.Utils;
using PirateX.Protocol;
using ProtoBuf;

namespace PirateX.Core.Actor
{
    public class NewSeed : RepAction<NewSeedResponse>
    {
        private static readonly bool[] CryptoByte = new bool[8]
        {
            false, false, false, false,
            false, false, false, true
        };
        public override NewSeedResponse Play()
        {
            var seed = Convert.ToInt32(Context.Request.QueryString["seed"]);

            var serverSeed = (int)(new DateTime(DateTime.Now.Ticks + RandomUtil.Random.Next(-10001, 10001)).GetTimestamp() / 1000);

            Context.ClientKeys = new KeyGenerator(seed).MakeKey();
            Context.ServerKeys = new KeyGenerator(serverSeed).MakeKey();
            Context.CryptoByte = new BitArray(CryptoByte).ConvertToByte();
            return new NewSeedResponse()
            {
                Seed = serverSeed
            };
        }
    }


    [ProtoContract]
    public class NewSeedResponse
    {
        [ProtoMember(1)]
        public int Seed { get; set; }
    }
}
