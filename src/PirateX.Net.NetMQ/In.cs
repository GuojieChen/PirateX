using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Net.NetMQ
{

    [ProtoContract]
    public class In
    {
        [ProtoMember(1)]
        public byte Version { get; set; }

        [ProtoMember(2)]
        public Action Action { get; set; }

        [ProtoMember(3)]
        public byte[] HeaderBytes { get; set; }

        [ProtoMember(4)]
        public byte[] QueryBytes { get; set; }

        [ProtoMember(5)]
        public string Ip { get; set; }

        [ProtoMember(6)]
        public int LastNo { get; set; }

        [ProtoMember(7)]
        public string SessionId { get; set; }
    }
}
