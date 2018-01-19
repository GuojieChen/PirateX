using System.Collections.Generic;
using System.Net;
using ProtoBuf;

namespace PirateX.Protocol
{

    [ProtoContract]
    public class In
    {
        [ProtoMember(1)]
        public byte Version { get; set; }

        [ProtoMember(2)]
        public PirateXAction Action { get; set; }

        [ProtoMember(3)]
        public byte[] HeaderBytes { get; set; } = new byte[0];

        [ProtoMember(4)]
        public byte[] QueryBytes { get; set; } = new byte[0];

        [ProtoMember(5)]
        public string Ip { get; set; }

        [ProtoMember(6)]
        public int LastNo { get; set; }

        [ProtoMember(7)]
        public string SessionId { get; set; }

        [ProtoMember(8)]
        public Dictionary<string,string> Items { get; set; }

        /// <summary>
        /// 打点
        /// </summary>
        [ProtoMember(9)]
        public Dictionary<string,string> Profile  = new Dictionary<string, string>();

        [ProtoMember(10)]
        public string ServerName { get; set; }

        public In()
        {
            ServerName = Dns.GetHostName();
        }
    }
}
