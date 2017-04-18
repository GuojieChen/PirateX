using System;
using ProtoBuf;

namespace PirateX.Core.Online
{
    [Serializable]
    [ProtoContract]
    public class OnlineRole  :IOnlineRole
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
        public string ResponseConvert { get; set; }
    }
}
