using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Domain.Entity;
using ProtoBuf;
using ServiceStack.DataAnnotations;

namespace GameServer.Console.SampleDomain
{
    [Serializable]
    [ProtoContract]
    public class Role:IEntity<int>
    {
        [ProtoMember(1)]
        [AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        [ProtoMember(2)]
        public DateTime CreateAt { get; set; }

        public long Vid { get; set; }

        public Role()
        {
            CreateAt = DateTime.UtcNow;
        }
    }
}
