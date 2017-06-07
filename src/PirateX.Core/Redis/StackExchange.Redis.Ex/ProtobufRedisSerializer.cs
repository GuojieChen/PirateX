using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Core.Redis.StackExchange.Redis.Ex
{
    public class ProtobufRedisSerializer : IRedisSerializer
    {
        public byte[] Serilazer<T>(T obj)
        {
            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                bytes = ms.ToArray();
            }

            return bytes;
        }

        public T Deserialize<T>(byte[] value)
        {
            if (value == null)
                return default(T);

            using (var ms = new MemoryStream(value))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}
