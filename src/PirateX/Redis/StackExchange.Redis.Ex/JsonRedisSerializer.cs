using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace PirateX.Redis.StackExchange.Redis.Ex
{
    public class JsonRedisSerializer : IRedisSerializer
    {
        public string Serilazer<T>(T obj)
        {
            return obj.ToJson();
        }

        public T Des<T>(string value)
        {
            return JsonSerializer.DeserializeFromString<T>(value); 
        }
    }
}
