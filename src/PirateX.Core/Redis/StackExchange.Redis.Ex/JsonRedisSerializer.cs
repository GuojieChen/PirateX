using System;
using System.Text;
using Newtonsoft.Json;

namespace PirateX.Core.Redis.StackExchange.Redis.Ex
{
    public class JsonRedisSerializer : IRedisSerializer
    {
        public byte[] Serilazer<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public T Deserialize<T>(byte[] value)
        {

            if (value == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value));
        }
    }
}
