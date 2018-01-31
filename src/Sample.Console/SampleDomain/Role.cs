using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core;
using ProtoBuf;
using ServiceStack.DataAnnotations;

namespace GameServer.Console.SampleDomain
{
    [Serializable]
    [ProtoContract]
    public class Role:IEntity<long>,IEntityTimestamp<byte[]>
    {
        [ProtoMember(1)]
        [AutoIncrement]
        public long Id { get; set; }

        public string Name { get; set; }

        [ProtoMember(2)]
        public string CreateUtcAt { get; set; }

        public Role()
        {
            CreateUtcAt = DateTime.UtcNow.FromDateTime();
        }
        
        public byte[] Timestamp { get; set; }

        public DateTimeOffset Tt { get; set; }
    }
}
