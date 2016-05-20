using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PirateX.Core.Redis.StackExchange.Redis.Ex;

namespace PirateX.Core.UnitTest.Redis.StackExchange.Redis.Ex
{
    [TestFixture]
    public class JsonRedisSerializerTest
    {
        JsonRedisSerializer serializer = new JsonRedisSerializer();


        [Test]
        public void Serializer_Object_OK()
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

            var json = serializer.Serilazer(obj); 

            Console.WriteLine(json);
            
            Assert.IsNotNull(json);
        }

        [Test]
        public void Deserialize_Object_Ok()
        {
            var jsonstr = "{\"Id\":1,\"Name\":\"Test\",\"Items\":[{\"Id\":1,\"Name\":\"Item_1\"},{\"Id\":2,\"Name\":\"Item_2\"},{\"Id\":3,\"Name\":\"Item_3\"},{\"Id\":4,\"Name\":\"Item_4\"}]}";

            var obj = serializer.Deserialize<SerializerObject>(jsonstr);

            Assert.IsNotNull(obj);
        }
    }
}
