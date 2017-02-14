using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ProtoBuf;

namespace PirateX.Core.UnitTest.Redis.StackExchange.Redis.Ex
{

    [Serializable]
    [ProtoContract(Name = "SerializerObject",SkipConstructor = true,IgnoreListHandling = true)]

    public class SerializerObject
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public IList<SerializerObjectItem> Items { get; set; }

        [ProtoMember(4)]
        public IList<int> List { get; set; }

        [ProtoMember(5)]
        public IDictionary<string,int> Dic { get; set; } = new Dictionary<string, int>();

        [ProtoMember(6)]
        public bool MyBoolean { get; set; } = true;

        [ProtoMember(7)]
        public IList<int> List2 { get; set; } = new List<int>();

        //[ProtoBeforeDeserialization]
        public void Init()
        {
            List = new List<int>();
            List.Add(-1);

        }

        public SerializerObject()
        {
            Init();
        }
    }

    [Serializable]
    [ProtoContract(Name = "SerializerObjectItem")]
    public class SerializerObjectItem
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        public SerializerObjectItem()
        {
            Id = -1;
            Name = "xxxx";
        }
    }
}
