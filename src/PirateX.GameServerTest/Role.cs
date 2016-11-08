using System;
using PirateX.Core.Domain.Entity;
using ProtoBuf;

namespace PirateX.GameServerTest
{
    [Serializable]
    [ProtoContract]
    public class Role:IEntity<long>,IEntityTimestamp<byte[]>
    {
        [ProtoMember(1)]
        public long Id { get; set; }

        public string Name { get; set; }

        [ProtoMember(2)]
        public DateTime CreateAt { get; set; }

        public Role()
        {
            CreateAt = DateTime.UtcNow;
        }
        
        public byte[] Timestamp { get; set; }
    }
}
