using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using ProtoBuf;

namespace PirateX.Core.UnitTest.Redis.StackExchange.Redis.Ex
{
    [TestFixture]
    public class ProtobufRedisSerializerTest
    {
        private IRedisSerializer serializer = new ProtobufRedisSerializer();

        [Test]
        public void Serializer_Deserialize_Object_OK()
        {

            var obj = new SerializerObject()
            {
                Id = 1,
                Name = "Test",
                Items = new List<SerializerObjectItem>()
                {
                    new SerializerObjectItem() {Id =1,Name = "Item_1" },
                    new SerializerObjectItem() {Id =2,Name = "Item_2" },
                    new SerializerObjectItem() {Id =3,Name = "Item_3" },
                    new SerializerObjectItem() {Id =4,Name = "Item_4" }
                }
            };

            var data = serializer.Serilazer(obj);

            Console.WriteLine(data);

            Assert.IsNotNull(data);

            var obj2 = serializer.Deserialize<SerializerObject>(data);

            Assert.IsNotNull(obj2);
        }

        [Test]
        ///http://stackoverflow.com/questions/1334659/how-do-i-generate-a-proto-file-from-a-c-sharp-class-decorated-with-attributes
        public void Generate_Proto()
        {
            var proto = Serializer.GetProto<SerializerObject>();
            
            Console.WriteLine(proto);
        }
    }
}
