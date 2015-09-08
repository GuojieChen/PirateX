using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Redis.StackExchange.Redis.Ex
{
    public interface IRedisSerializer
    {
        string Serilazer<T>(T obj);

        T Des<T>(string value);
    }
}
