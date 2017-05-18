using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Net.NetMQ
{
    [ProtoContract]
    public class Out
    {
        [ProtoMember(1)]
        public byte Version { get; set; }

        [ProtoMember(2)]
        public Action Action { get; set; }

        [ProtoMember(3)]
        public string SessionId { get; set; }

        [ProtoMember(4)]
        public int LastNo { get; set; }

        [ProtoMember(5)]
        public byte[] HeaderBytes { get; set; }

        [ProtoMember(6)]
        public byte[] BodyBytes { get; set; }

        [ProtoMember(7)]
        public int Id { get; set; }

        [ProtoMember(8)]
        public byte[] ClientKeys { get; set; }

        [ProtoMember(9)]
        public byte[] ServerKeys { get; set; }

        [ProtoMember(10)]
        public byte Crypto { get; set; }
    }
}
