using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using ProtoBuf;
using ServiceStack;
using StackExchange.Redis;

namespace PirateX.Core.UnitTest.Redis.StackExchange.Redis.Ex
{
    [TestFixture]
    public class ProtobufRedisSerializerTest
    {
        private ProtobufRedisSerializer serializer = new ProtobufRedisSerializer();

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

            Console.WriteLine(JsonConvert.SerializeObject(obj));
            Console.WriteLine(JsonConvert.SerializeObject(obj2));
        }

        [Test]
        ///http://stackoverflow.com/questions/1334659/how-do-i-generate-a-proto-file-from-a-c-sharp-class-decorated-with-attributes
        public void Generate_Proto()
        {
            var proto = Serializer.GetProto<SerializerObject>();
            
            Console.WriteLine(proto);
        }

        [Test]
        public void Redis_Serializer_Deserialize_Object()
        {
            var db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
            var urn = "test"; //Guid.NewGuid().ToString();

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
                },
                List = new List<int>()
                {
                    3,4
                },
                MyBoolean = false,
                //Dic = new Dictionary<string, int>()
                //{
                //    {"A",1 },
                //    {"B",2 }
                //}
            };


            var data = serializer.Serilazer(obj);

            //using (var ms = new MemoryStream())
            //{
            //    Serializer.Serialize(ms, obj);
            //    db.StringSet(urn, ms.ToArray());
            //}


            db.StringSet(urn, data);

            var dataFromRedis = db.StringGet(urn);
            db.StringSet($"urn_json", obj.ToJson());
            Console.WriteLine("Json");
            Console.WriteLine(db.StringGet($"urn_json"));

            //Assert.AreEqual(data,dataFromRedis);

            //Console.WriteLine(data);

            using (var ms = new MemoryStream(dataFromRedis))
            {
                var obj2 = Serializer.Deserialize<SerializerObject>(ms);

                //Console.WriteLine(serializer.Serilazer(obj2));

                Console.WriteLine("Protobuf");
                Console.WriteLine(obj2.ToJson());

            }



        }

        [Test]
        public void serializ_datetime()
        {
            var obj = new SerializerObject() { CreateAt = DateTime.UtcNow, Id = 2 };

            var bytes = serializer.Serilazer(obj);
            Console.WriteLine(obj.CreateAt);
            Console.WriteLine(Encoding.UTF8.GetString(bytes));

            var obj2 = serializer.Deserialize<SerializerObject>(bytes);

            Console.WriteLine(obj2.CreateAt);

        }

        [Test]
        public void print_test()
        {
            Console.WriteLine("a");
            print();
            Console.WriteLine("b");
        }

        public async Task print()
        {
            Console.WriteLine("1");
            Thread.Sleep(3000);

            await subprint();
            Console.WriteLine("2");
        }

        public async Task subprint()
        {
            Console.WriteLine("3");
            Thread.Sleep(3000);
            Console.WriteLine("4");
        }
    }
}
