using System;
using System.ComponentModel.DataAnnotations;
using PirateX.Core;
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


        public Role()
        {
            CreateUtcAt = DateTime.UtcNow;
        }
        
        public byte[] Timestamp { get; set; }
        public DateTime CreateUtcAt { get; set; }
    }
}
