using System;
using System.Linq;
using StackExchange.Redis;

namespace PirateX.Core.Redis.StackExchange.Redis.Ex
{
    public static class RedisDataBaseExtension
    {
        private static IRedisSerializer _redisSerilazer;

        public static IRedisSerializer RedisSerilazer
        {
            get
            {
                if(_redisSerilazer == null)
                    _redisSerilazer = new JsonRedisSerializer();
                return _redisSerilazer;
            }
            set
            {
                if (value != null)
                    _redisSerilazer = value; 
            }
        }


        public static T Get<T>(this IDatabase db, string key)
        {
            var value = db.StringGet(key);
            if (string.IsNullOrEmpty(value))
                return default(T);
            return RedisSerilazer.Deserialize<T>(value); 
        }

        public static bool Set<T>(this IDatabase db,RedisKey key, T t,TimeSpan? expire = null)
        {
            var value = RedisSerilazer.Serilazer<T>(t);
            if (value == null)
                return false;

            return db.StringSet(key, value, expire);
        }

        public static bool Set<T>(this IDatabase db, T t, TimeSpan? expire = null)
        {
            return db.Set(GetIdAsKey(t), t, expire);
        }
        
        public static void SetAsHash<T>(this IDatabase db, T t,TimeSpan? expire = null)
        {
            var key = GetIdAsKey(t);
            
            db.HashSet(key, t.GetType().GetProperties().Select(property => new HashEntry(property.Name, property.GetValue(t).ToString())).ToArray());
            // TODO 缓存时间
        }

        private static string GetIdAsKey<T>(T t)
        {
            object idValue = typeof(T).GetProperty("Id").GetValue(t);

            return idValue?.ToString() ?? "";
        }
    }
}
