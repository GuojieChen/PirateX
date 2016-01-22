using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX;
using PirateX.Filters;
using PirateX.Protocol;
using ProtoBuf;

namespace GameServer.Console.Cmd
{
    [AuthorizedFilterBase]
    public class RoleInfo: GameCommand<DemoSession,RoleInfoRequest,RoleInfoResponse>
    {
        protected override RoleInfoResponse ExecuteResponseCommand(DemoSession session, RoleInfoRequest data)
        {
            return new RoleInfoResponse()
            {
                Name = "mrglee",
                Lv = 2 ,
                CreateAt = DateTime.UtcNow
            };
        }
    }

    public class RoleInfoRequest
    {
        
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
        public DateTime CreateAt { get; set; }

        [ProtoMember(4)]
        public TestData Data { get; set; }

        [ProtoMember(5)]
        public List<string> List { get; set; }

        [ProtoMember(6)]
        public List<TestData> Datas { get; set; } 
    }

    [Serializable]
    [ProtoContract(Name = "TestData")]
    public class TestData
    {
        [ProtoMember(1)]
        public int Age { get; set; }
    }
}
