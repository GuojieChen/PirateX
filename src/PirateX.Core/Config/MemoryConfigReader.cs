using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PirateX.Core
{
    public class MemoryConfigReader : IConfigReader
    {
        private readonly MemoryCacheClient _cacheClient = new MemoryCacheClient();

        private readonly IDictionary<string, MemoryCacheClient> Cache = new Dictionary<string, MemoryCacheClient>();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<Assembly> _configAssembly;

        private bool isLoaded;

        private object _lockHelper = new object();

        private IConfigProvider _configProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="dbConnection"></param>
        public MemoryConfigReader(List<Assembly> assemblies, IConfigProvider provider)
        {
            _configProvider = provider;
            _configAssembly = assemblies;
        }

        public void Load()
        {
            if (isLoaded)
                return;

            lock (_lockHelper)
            {
                if (isLoaded)
                    return;
                var types = new List<Type>();
                _configAssembly.ForEach(a => types.AddRange(a.GetTypes()
                    .Where(item => typeof(IConfigEntity).IsAssignableFrom(item))));


                var queue = new Queue<Type>(types);
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
                                                        .Invoke(this, null);
                            }
                            else //if (typeof(IConfigIdEntity).IsAssignableFrom(type))
                            {
                                this.GetType().GetMethod("LoadConfigData", BindingFlags.Instance | BindingFlags.NonPublic)
                                                        .MakeGenericMethod(type)
                                                        .Invoke(this,null);
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


        private void LoadConfigData<T>() where T : IConfigIdEntity
        {
            var list = _configProvider.LoadConfigData<T>();
            if (list == null)
                return;
            var type = typeof(T);

            var listkeys = new List<string>();

            foreach (var item in list)
            {
                if (item == null)
                    continue;
                var key = GetCacheKeyId<T>(item.Id);
                _cacheClient.Set(key, item);
                listkeys.Add(key);

                foreach (var attr in type.GetCustomAttributes().Where(x => x is ConfigIndex))
                {
                    var configindex = (ConfigIndex)attr;
                    //获取 对应的字段列表
                    var ps = type.GetProperties().Where(p => configindex.Names.Contains(p.Name));
                    var dic = ps.ToDictionary(info => info.Name, info => info.GetValue(item));


                    if (configindex.IsUnique)
                    {
                        var urn = GetCacheIndexKey<T>(dic);
                        //这里只存根据ID生成的KEY
                        _cacheClient.Set(urn, key);
                    }
                    else
                    {
                        var urn = GetCacheIndexKey2<T>(dic);
                        var urnlist = _cacheClient.Get<List<string>>(urn) ?? new List<string>();

                        urnlist.Add(key);

                        _cacheClient.Set(urn, urnlist);
                    }
                }
            }

            if (listkeys.Any())
                _cacheClient.Add(GetCacheKeyListKey<T>(), listkeys);
        }

        private void LoadKeyValueConfigData<T>() where T : IConfigKeyValueEntity
        {
           var list = _configProvider.LoadConfigData<T>();

                foreach (var item in list)
                {
                    if (item == null)
                        continue;

                    var key = GetCacheKey<T>(item.Id);
                    _cacheClient.Set(key, item.Value);
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
        private static string GetCacheIndexKey<T>(IDictionary<string, object> dic) where T : IConfigIdEntity
        {
            var builder = new StringBuilder("sysconfig:_unique_:");
            builder.Append(typeof(T).Name);
            builder.Append(":");
            bool isFirst = true;
            foreach (var keyValue in dic.OrderBy(item => item.Key))
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

        private static string GetCacheIndexKey2<T>(IDictionary<string, object> dic) where T : IConfigIdEntity
        {
            var builder = new StringBuilder("sysconfig:_indexes_:");
            builder.Append(typeof(T).Name);
            builder.Append(":");
            bool isFirst = true;
            foreach (var keyValue in dic.OrderBy(item => item.Key))
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
            var listkeys = _cacheClient.GetKeys(GetCacheKeyListKey<T>());
            if (listkeys != null)
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
            if (!string.IsNullOrEmpty(key))
                return _cacheClient.Get<T>(key);
            return default(T);
        }

        public IEnumerable<T> SelectByIndexes<T>(object index) where T : IConfigIdEntity
        {
            var dic = index.ToDictionary();
            var urn = GetCacheIndexKey2<T>(dic);

            var keys = _cacheClient.Get<IList<string>>(urn);
            if (keys != null)
                return _cacheClient.GetAll<T>(keys).Values;

            return new T[0];

        }
        #endregion
    }
}
