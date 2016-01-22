using System;
using Newtonsoft.Json;

namespace PirateX.Core.Redis.StackExchange.Redis.Ex
{
    public class JsonRedisSerializer : IRedisSerializer
    {
        public string Serilazer<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
