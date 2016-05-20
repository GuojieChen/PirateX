using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Core.UnitTest.Redis.StackExchange.Redis.Ex
{

    [Serializable]
    [ProtoContract(Name = "SerializerObject")]

    public class SerializerObject
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public IList<SerializerObjectItem> Items { get; set; }  
    }

    [Serializable]
    [ProtoContract(Name = "SerializerObjectItem")]
    public class SerializerObjectItem
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }
    }
}
