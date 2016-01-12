using System;
using ProtoBuf;

namespace PirateX.Core.Online
{
    [Serializable]
    [ProtoContract]
    public class OnlineRole  :IOnlineRole
    {
        [ProtoMember(1)]
        public long Id { get; set; }
        [ProtoMember(2)]
        public int Did { get; set; }
        [ProtoMember(3)]
        public string HotName { get; set; }
        [ProtoMember(4)]
        public string SessionID { get; set; }
    }
}
