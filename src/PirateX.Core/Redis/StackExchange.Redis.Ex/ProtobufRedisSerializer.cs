using System.IO;
using ProtoBuf;

namespace PirateX.Core
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
