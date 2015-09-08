using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Redis.StackExchange.Redis.Ex
{
    public class ProtobufRedisSerializer : IRedisSerializer
    {

        public string Serilazer<T>(T obj)
        {
            byte[] bytes = null; 
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                bytes = ms.ToArray();
            }

            return Encoding.Default.GetString(bytes); 
        }

        public T Des<T>(string value)
        {
            var bytes = Encoding.Default.GetBytes(value); 

            using (var ms = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
