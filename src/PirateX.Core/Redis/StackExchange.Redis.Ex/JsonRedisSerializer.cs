using System;

namespace PirateX.Core.Redis.StackExchange.Redis.Ex
{
    public class JsonRedisSerializer : IRedisSerializer
    {
        public string Serilazer<T>(T obj)
        {
            throw new NotImplementedException();
            
            //return obj.ToJson();
        }

        public T Deserialize<T>(string value)
        {
            throw new NotImplementedException();

            //return JsonSerializer.DeserializeFromString<T>(value); 
        }
    }
}
