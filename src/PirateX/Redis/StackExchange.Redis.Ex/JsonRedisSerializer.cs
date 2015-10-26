using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Redis.StackExchange.Redis.Ex
{
    public class JsonRedisSerializer : IRedisSerializer
    {
        public string Serilazer<T>(T obj)
        {
            throw new NotImplementedException();
            
            //return obj.ToJson();
        }

        public T Des<T>(string value)
        {
            throw new NotImplementedException();

            //return JsonSerializer.DeserializeFromString<T>(value); 
        }
    }
}
