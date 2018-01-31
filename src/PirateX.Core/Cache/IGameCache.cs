using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace PirateX.Core
{
    public interface IGameCache
    {
        void Store<T>(T t,object id) where T : IEntityPrivate;

        void Remove<T>(long rid, object id) where T : IEntityPrivate;

        T Single<T>(long rid,object id) where T : IEntityPrivate;

        IEnumerable<T> GetList<T>(long rid);
    }

    public class DefaultGameCache : IGameCache
    {
        private IDatabase _redis;
        private IRedisSerializer _redisSerializer;

        public DefaultGameCache(IRedisSerializer redisSerializer, IDatabase redis)
        {
            _redis = redis;
            _redisSerializer = redisSerializer;
        }

        private string GetItemKey<T>(object id) where T :IEntityPrivate
        {
            return $"{typeof(T).Name.ToLower()}:{id}";
        }

        private string GetSetKey<T>(long rid) 
        {
            return $"{typeof(T).Name.ToLower()}_list:{rid}";
        }
        public void Store<T>(T t, object id) where T : IEntityPrivate
        {
            var itemkey = GetItemKey<T>(id);
            var tran = _redis.CreateTransaction();
            tran.StringSetAsync(itemkey, _redisSerializer.Serilazer(t));
            tran.SetAddAsync(GetSetKey<T>(t.Rid),itemkey);
            tran.Execute();
        }

        public void Remove<T>(long rid,object id) where T : IEntityPrivate
        {
            var itemkey = GetItemKey<T>(id);

            var tran = _redis.CreateTransaction();
            tran.KeyDeleteAsync(itemkey);
            tran.SetRemoveAsync(GetSetKey<T>(rid), itemkey);
            tran.Execute();
        }

        public T Single<T>(long rid,object id) where T : IEntityPrivate
        {
            var setkey = GetSetKey<T>(rid);
            if (!_redis.KeyExists(setkey))
                return default(T);

            return _redisSerializer.Deserialize<T>(_redis.StringGet(GetItemKey<T>(id))); 
        }

        public IEnumerable<T> GetList<T>(long rid)
        {
            var values = _redis.SetMembers(GetSetKey<T>(rid));

            return _redis.StringGet(values.Select(item => (RedisKey) Convert.ToString(item)).ToArray()).Select(item=>_redisSerializer.Deserialize<T>(item)); 
        }
    }
}
