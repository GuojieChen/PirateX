using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using StackExchange.Redis;

namespace PirateX.Core.Actor
{
    public interface IReqCache
    {
        TResponse GetFromCache<TResponse>(string key);

        void SetToCache<TResponse>(string listkey, string key, TResponse response);
    }


    public class MemoryReqCache : IReqCache
    {
        public TResponse GetFromCache<TResponse>(string key)
        {
            throw new NotImplementedException();
        }

        public void SetToCache<TResponse>(string key, TResponse response)
        {
        }

        public void SetToCache<TResponse>(string listkey, string key, TResponse response)
        {
        }
    }

    public class RedisReqCache : IReqCache
    {
        private IRedisSerializer RedisSerializer;
        private ConnectionMultiplexer _connectionMultiplexer;

        public TResponse GetFromCache<TResponse>(string key)
        {
            var redis = _connectionMultiplexer.GetDatabase();

            var data = redis.StringGet(key);
            return RedisSerializer.Deserialize<TResponse>(data);
        }

        public void SetToCache<TResponse>(string listkey, string key, TResponse response)
        {
            var listurn = listkey;
            if (string.IsNullOrEmpty(listurn))
                return;

            var redis = _connectionMultiplexer.GetDatabase();

            var trans = redis.CreateTransaction();
            trans.StringSetAsync(key, RedisSerializer.Serilazer(response), new TimeSpan(0, 0, 30));
            trans.ListRightPushAsync(listurn, key);
            trans.Execute();

            if (redis.ListLength(listurn) >= 4)
            {//保存4条
                var removekey = redis.ListLeftPop(listurn);
                redis.KeyDelete(removekey.ToString());
            }
        }
    }
}
