using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Net.NNanomsg
{
    [ProtoContract]
    public class GmaeMail
    {
        [ProtoMember(1)]
        public byte Version { get; set; }

        [ProtoMember(2)]
        public int Action { get; set; }

        [ProtoMember(3)]
        public string SessionId { get; set; }

        [ProtoMember(4)]
        public byte[] ClientKeys { get; set; }

        [ProtoMember(5)]
        public byte[] ServerKeys { get; set; }

        [ProtoMember(6)]
        public byte[] Headers { get; set; }

        [ProtoMember(7)]
        public byte[] Body { get; set; }

    }
}
