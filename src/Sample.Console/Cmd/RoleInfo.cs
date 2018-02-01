using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using PirateX;
using PirateX.Core;
using PirateX.Protocol;
using ProtoBuf;
using StackExchange.Redis;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Console.Cmd
{
    public class RoleInfo: RepAction<RoleInfoResponse>
    {
        public override RoleInfoResponse Play()
        {
            //MessageSender.PushMessage(Context.Token.Rid,new{A="111",B="222"});

            base.ServerReslover.Resolve<IDatabase>().StringSet("a", DateTime.Now.Ticks);

            return new RoleInfoResponse()
            {
                Name = "mrglee",
                Lv = 2,
                CreateAt = DateTime.UtcNow.FromDateTime(),
                A = new DateTime(Convert.ToInt64(ServerReslover.Resolve<IDatabase>().StringGet("a")))
            };
        }
    }


    [Serializable]
    [ProtoContract(Name = "RoleInfoResponse")]
    public class RoleInfoResponse
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int Lv { get; set; }

        [ProtoMember(3)]
        public string CreateAt { get; set; }

        [ProtoMember(4)]
        public TestData Data { get; set; }

        [ProtoMember(5)]
        public List<string> List { get; set; }

        [ProtoMember(6)]
        public List<TestData> Datas { get; set; } 

        public DateTime A { get; set; }
    }

    [Serializable]
    [ProtoContract(Name = "TestData")]
    public class TestData
    {
        [ProtoMember(1)]
        public int Age { get; set; }
    }
}
