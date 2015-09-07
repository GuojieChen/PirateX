﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace PirateX.Redis.StackExchange.Redis.Ex
{
    public static class RedisDataBaseExtension
    {
        private static IRedisSerilazer redisSerilazer; 

        public static T Get<T>(this IDatabase db, string key)
        {
            var value = db.StringGet(key);
            if (string.IsNullOrEmpty(value))
                return default(T);
            return redisSerilazer.Des<T>(value); 
        }

        public static void Set<T>(this IDatabase db,RedisKey key, T t,TimeSpan? expire = null)
        {
            var value = redisSerilazer.Serilazer<T>(t);
            if (string.IsNullOrEmpty(value))
                return;

            db.StringSet(key, value, expire);
        }
    }
}
