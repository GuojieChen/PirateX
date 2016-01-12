using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using PirateX.Core.Cache;

namespace PirateX.Core.Config
{
    public class MemoryConfigReader : IConfigReader
    {
        private readonly MemoryCacheClient _cacheClient = new MemoryCacheClient();

        private readonly Assembly _configAssembly;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configAssembly">配置模型所在的程序集</param>
        public MemoryConfigReader(Assembly configAssembly)
        {
            _configAssembly = configAssembly;
        }

        public void Load(IDatabaseFactory connection)
        {
            foreach (var type in _configAssembly.GetTypes())
            {     
                if (typeof(IConfigKeyValueEntity).IsAssignableFrom(type))
                {
                    this.GetType().GetMethod("LoadKeyValueConfigData", BindingFlags.Instance | BindingFlags.NonPublic)
                                            .MakeGenericMethod(type)
                                            .Invoke(this, new object[] { connection });
                }
                else if (typeof(IConfigEntity).IsAssignableFrom(type))
                {
                    this.GetType().GetMethod("LoadConfigData", BindingFlags.Instance | BindingFlags.NonPublic)
                                            .MakeGenericMethod(type)
                                            .Invoke(this, new object[] { connection });
                }
            }
        }

        private void LoadConfigData<T>(IDatabaseFactory connection) where T : IConfigIdEntity
        {
            var list = connection.Select<T>();

            if (!list.Any())
                return;

            var listkeys = new List<string>();
            var indexkeys = new List<string>();

            foreach (var item in list)
            {
                if (item == null)
                    continue;
                var key = GetCacheKeyId<T>(item.Id);
                _cacheClient.Add(key, item);
                listkeys.Add(key);

                foreach (var attr in item.GetType().GetCustomAttributes().Where(x=>x.GetType() == typeof(ConfigIndex)))
                {// 添加索引
                    //TODO ~~~~
                }
            }

            if (listkeys.Any())
                _cacheClient.Add(GetCacheKeyListKeys<T>() , listkeys);

            if (indexkeys.Any())
                _cacheClient.Add(GetCacheKeyIndexKeys<T>(),indexkeys);
        }

        private void LoadKeyValueConfigData<T>(IDatabaseFactory connection) where T : IConfigKeyValueEntity
        {
            
            var list = connection.Select<T>();

            if (!list.Any())
                return;

            foreach (var item in list)
            {
                if (item == null)
                    continue;

                var key = GetCacheKey<T>(item.Id);
                _cacheClient.Add(key, item.V); 
            }
        }

        private string GetCacheKeyId<T>(object id)
        {
            return $"id:{typeof(T).Name}:{id}"; 
        }

        private string GetCacheKeyListKeys<T>()
        {
            return $"list:{typeof (T).Name}";
        }

        private string GetCacheKeyIndexKeys<T>()
        {
            return $"index:{typeof (T).Name}";
        }

        private string GetCacheKey<T>(string key)
        {
            return $"kv:{typeof (T).Name}:{key}"; 
        }

        public T SingleById<T>(object id) where T : IConfigEntity
        {
            return _cacheClient.Get<T>(GetCacheKeyId<T>(id)); 
        }

        public IEnumerable<T> Select<T>() where T : IConfigEntity
        {
            var listkeys = _cacheClient.Get<IList<string>>(GetCacheKeyListKeys<T>());
            if (listkeys.Any())
                return _cacheClient.GetAll<T>(listkeys).Values;

            return new T[0];
        }

        public TValue GetValue<T, TValue>(string key)
        {
            return _cacheClient.Get<TValue>(GetCacheKey<T>(key));
        }

        public T SingleByIndexes<T>(object index)
        {
            throw new NotImplementedException();
        }
    }
}
