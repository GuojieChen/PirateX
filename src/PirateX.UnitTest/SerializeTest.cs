using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using ProtoBuf;
using ServiceStack.Text;
using TestDataGenerator;
using SerializationContext = MsgPack.Serialization.SerializationContext;

namespace PirateX.UnitTest
{
    [TestFixture]
    public class SerializeTest
    {
        [Test]
        public void Test()
        {
            var catalog = new Catalog();

            IList<Pet2> pets = new List<Pet2>();
            for (int i = 0; i < 2000; i++)
            {
                pets.Add(catalog.CreateInstance<Pet2>());
            }

            var serializer = SerializationContext.Default.GetSerializer<IList<Pet2>>();

            byte[] datas = null;
            using (var stream = new MemoryStream())
            {
                serializer.Pack(stream, pets);
                datas = stream.ToArray();
            }

            $"MsgPack Len  :{datas.Length}".PrintDump();

            //var serializer2 = SerializationContext.Default.GetSerializer<IList<Pet>>();
            //using (var stream = new MemoryStream(datas))
            //{
            //    var list = serializer2.Unpack(stream);

            //    list.PrintDump();
            //}


            var jsonDatas = JsonConvert.SerializeObject(pets);

            $"Json.NET Len  :{jsonDatas.Length}".PrintDump();

            byte[] protoDatas = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, pets);

                protoDatas = ms.ToArray();
            }

            $"protobuf-net Len  :{protoDatas.Length}".PrintDump();

            using (var stream = new MemoryStream(protoDatas))
            {
                var list = Serializer.Deserialize<IList<Pet2>>(stream);

                list.PrintDump();
            }

        }
    }


    [Serializable]
    [ProtoContract(Name = "Pet")]
    public class Pet
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public IList<string> Datas { get; set; }

        [ProtoMember(4)]
        public int Atk { get; set; }

        [ProtoMember(5)]
        public int Def { get; set; }
    }


    [Serializable]
    [ProtoContract(Name = "Pet")]
    public class Pet2
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public IList<string> Datas { get; set; }

        [ProtoMember(4)]
        public int Atk { get; set; }

        [ProtoMember(5)]
        public int Def { get; set; }

        public string Ex { get; set; }

        public IList<string> Args { get; set; }
    }
}
