using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PirateX.Core.Cache;
using PirateX.Core.Utils;

namespace PirateX.Core.Config
{
    public class MemoryConfigReader : IConfigReader
    {
        private readonly MemoryCacheClient _cacheClient = new MemoryCacheClient();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Assembly _configAssembly;

        private bool isLoaded;

        private object _lockHelper = new object();

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
            if (isLoaded)
                return;

            lock (_lockHelper)
            {
                if (isLoaded)
                    return;

                var queue = new Queue<Type>(_configAssembly.GetTypes().Where(item => typeof(IConfigEntity).IsAssignableFrom(item)));
                if (Logger.IsTraceEnabled)
                    Logger.Trace($"Loading config datas({queue.Count})");
                while (queue.Any())
                {
                    var tasks = new List<Task>();
                    for (var i = 0; i < 5 && queue.Any(); i++)
                    {
                        var type = queue.Dequeue();

                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            if (typeof(IConfigKeyValueEntity).IsAssignableFrom(type))
                            {
                                this.GetType().GetMethod("LoadKeyValueConfigData", BindingFlags.Instance | BindingFlags.NonPublic)
                                                        .MakeGenericMethod(type)
                                                        .Invoke(this, new object[] { connection });
                            }
                            else if (typeof(IConfigIdEntity).IsAssignableFrom(type))
                            {
                                this.GetType().GetMethod("LoadConfigData", BindingFlags.Instance | BindingFlags.NonPublic)
                                                        .MakeGenericMethod(type)
                                                        .Invoke(this, new object[] { connection });
                            }
                        }));
                    }

                    try
                    {
                        Task.WaitAll(tasks.ToArray());
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception);
                        throw;
                    }
                }

                isLoaded = true;
            }
        }
        #region Load Methods
        private void LoadConfigData<T>(IDatabaseFactory connection) where T : IConfigIdEntity
        {
            var list = connection.Select<T>();

            if (!list.Any())
                return;

            var type = typeof (T);

            var listkeys = new List<string>();

            foreach (var item in list)
            {
                if (item == null)
                    continue;
                var key = GetCacheKeyId<T>(item.Id);
                _cacheClient.Set(key, item);
                listkeys.Add(key);

                foreach (var attr in type.GetCustomAttributes().Where(x=>x.GetType() == typeof(ConfigIndex)))
                {// 添加索引
                    var configindex = (ConfigIndex) attr;
                    //获取 对应的字段列表
                    var ps = type.GetProperties().Where(p => configindex.Names.Contains(p.Name));
                    var dic = ps.ToDictionary(info => info.Name, info => info.GetValue(item));
                    //这里只存根据ID生成的KEY
                    _cacheClient.Set(GetCacheIndexKey<T>(dic), key);
                }
            }

            if (listkeys.Any())
                _cacheClient.Add(GetCacheKeyListKey<T>() , listkeys);
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
                _cacheClient.Set(key, item.V); 
            }
        }
        #endregion

        #region Caching Names
        /// <summary> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetCacheKeyId<T>(object id)
        {
            var key = $"systemconfig:_id_:{typeof(T).Name}:{id}";
            return key; 
        }

        private static string GetCacheKeyListKey<T>()
        {
            var key = $"systemconfig:_list_:{typeof(T).Name}";
            return key;
        }
        /// <summary> 按照名称进行升序排序，中间 “_”拼接，获得KEY值
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        private static string GetCacheIndexKey<T>(IDictionary<string,object> dic) where T : IConfigIdEntity
        {
            var builder = new StringBuilder("systemconfig:_indexes_:");
            builder.Append(typeof (T).Name);
            builder.Append(":");
            bool isFirst = true;
            foreach (var keyValue in dic.OrderBy(item=>item.Key))
            {
                if (!isFirst)
                {
                    builder.Append("_");
                }
                isFirst = false;
                builder.Append(keyValue.Value);
            }
            return builder.ToString();
        }


        private static string GetCacheKey<T>(string key)
        {
            var key2 = $"systemconfig:_kv_:{typeof(T).Name}:{key}";
            return key2;
        }
        #endregion

        #region Caching Methods

        public T SingleById<T>(object id) where T : IConfigEntity
        {
            return _cacheClient.Get<T>(GetCacheKeyId<T>(id)); 
        }

        public IEnumerable<T> Select<T>() where T : IConfigEntity
        {
            var listkeys = _cacheClient.Get<IList<string>>(GetCacheKeyListKey<T>());
            if (listkeys.Any())
                return _cacheClient.GetAll<T>(listkeys).Values;

            return new T[0];
        }

        public TValue GetValue<T, TValue>(string key)
        {
            return _cacheClient.Get<TValue>(GetCacheKey<T>(key));
        }

        public T SingleByIndexes<T>(object index) where T : IConfigIdEntity
        {
            var dic = index.ToDictionary();
            var indexkey = GetCacheIndexKey<T>(dic);

            var key = _cacheClient.Get<string>(indexkey);
            if(!string.IsNullOrEmpty(key))
                return _cacheClient.Get<T>(key);
            return default(T);
        }

        #endregion
    }
}
