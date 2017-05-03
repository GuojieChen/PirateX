using System;
using System.Collections.Generic;
using ProtoBuf;

namespace PirateX.Core.Session
{
    [Serializable]
    [ProtoContract]
    public class PirateSession 
    {
        [ProtoMember(1)] //role id
        public long Id { get; set; }
        [ProtoMember(2)]
        public int Did { get; set; }
        [ProtoMember(3)]
        public string SessionId { get; set; }
        [ProtoMember(4)]
        public DateTime StartUtcAt { get; set; }
        [ProtoMember(5)]
        public string Token { get; set; }
        [ProtoMember(6)]
        public string Uid { get; set; }
        [ProtoMember(7)]
        public byte[] ClientKeys { get; set; }
        [ProtoMember(8)]
        public byte[] ServerKeys { get; set; }
        [ProtoMember(9)]
        public byte CryptoByte { get; set; }

        [ProtoMember(10)]
        public string ResponseConvert { get; set; }

        [ProtoMember(11)]
        public DateTime LastUtcAt { get; set; }

        [ProtoMember(12)]
        public IDictionary<string, string> Items { get; set; }
    }
}
