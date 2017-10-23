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
        public int Id { get; set; }
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
        public long LastUtcAt { get; set; }

        public DateTime GetLastAtUtc()
        {
            return new DateTime(LastUtcAt,DateTimeKind.Utc);
        }

        [ProtoMember(12)]
        public IDictionary<string, string> Items { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 角色创建时间
        /// </summary>
        [ProtoMember(13)]
        public long CreateAtUtc { get; set; }

        public DateTime GetCreateAtUtc()
        {
            return new DateTime(CreateAtUtc,DateTimeKind.Utc);
        }

        [ProtoMember(14)]
        public string ServerName { get; set; }
    }
}
