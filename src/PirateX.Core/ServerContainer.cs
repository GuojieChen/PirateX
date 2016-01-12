using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using NLog;
using PirateX.Core.Config;
using PirateX.Core.Domain.Entity;
using PirateX.Core.Utils;
using StackExchange.Redis;

namespace PirateX.Core
{
    /// <summary> 默认的游戏容器实现
    /// </summary>
    public abstract class ServerContainer<TDistrictConfig> : IServerContainer<TDistrictConfig> where TDistrictConfig : IDistrictConfig
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary> 容器集合
        /// </summary>
        private readonly IDictionary<int, IContainer> _containers = new SortedDictionary<int, IContainer>();

        private readonly object _loadContainerLockHelper = new object();

        public ILifetimeScope ServerIoc { get; set; }

        public IServerSetting Settings { get;  }

        public ServerContainer(IServerSetting settings)
        {
            Settings = settings;

            if (Settings == null)
            {
                if (Log.IsTraceEnabled)
                    Log.Trace("Settings is NULL");

                Settings = GetDefaultSeting();
            }


            if (Log.IsTraceEnabled)
            {
                Log.Trace("Settings values:");
                Log.Trace(Settings.ToLogString());
            }
        }

        public ServerContainer():this(null)
        {
            
        }

        private static DefaultServerSetting GetDefaultSeting()
        {
            var ps = typeof (DefaultServerSetting).GetProperties();
            var defaultServerSetting = Activator.CreateInstance(typeof(DefaultServerSetting), null);

            foreach (var propertyInfo in ps)
            {
                var value = System.Configuration.ConfigurationManager.AppSettings.Get(propertyInfo.Name.ToLower());
                propertyInfo.SetValue(defaultServerSetting,value);
            }

            return (DefaultServerSetting)defaultServerSetting;
        }

        public IContainer GetDistrictContainer(int districtid)
        {
            if (_containers.ContainsKey(districtid))
                return _containers[districtid];

            lock (_loadContainerLockHelper)
            {
                if (_containers.ContainsKey(districtid))
                    return _containers[districtid];

                var c = LoadDistrictContainer(GetDistrictConfig(districtid));
                if (c == null)
                    return null;

                _containers.Add(districtid, c);

                return c;
            }
        }

        public IContainer ReLoadContainer(int districtid)
        {
            var districtConfig = GetDistrictConfig(districtid); 

            var c = LoadDistrictContainer(districtConfig);

            if (_containers.ContainsKey(districtid))
                _containers[districtid] =c ;
            else 
                _containers.Add(districtid,c);

            return c;
        }

        public IEnumerable<TDistrictConfig> GetDistrictConfigs()
        {
            var list = LoadDistrictConfigs();
            if (Settings.Districts == null)
                return list;

            return list.Where(item => Settings.Districts.Select(d=>d.ServerId).Contains(item.Id));
        }

        private IContainer LoadDistrictContainer(TDistrictConfig districtConfig)
        {
            if (districtConfig == null)
                return null;

            if(Log.IsTraceEnabled)
                Log.Trace($"Init district container\t{districtConfig.Id}");

            var builder = new ContainerBuilder();

            builder.Register(c => districtConfig).As<TDistrictConfig>().SingleInstance();
            builder.Register(c => ConnectionMultiplexer.Connect(districtConfig.Redis));


            builder.Register(c => c.Resolve<ConnectionMultiplexer>().GetDatabase(districtConfig.RedisDb)).As<IDatabase>();
            builder.Register(c => districtConfig).As<IDistrictConfig>().SingleInstance();
            BuildContainer(builder, districtConfig);

            //默认Config缓存数据处理器
            builder.Register(c => new MemoryConfigReader(ServerIoc.ResolveNamed<Assembly>("ConfigAssembly")))
                .As<IConfigReader>()
                .SingleInstance();


            var container = builder.Build();

            if(Log.IsTraceEnabled)
                Log.Trace("CreateAndAlterTable");
            if (ServerIoc.IsRegisteredWithName<Assembly>("EntityAssembly") && Settings.AlterTable && districtConfig.AlterTable )
                container.Resolve<IDatabaseFactory>().CreateAndAlterTable(ServerIoc.ResolveNamed<Assembly>("EntityAssembly").GetTypes().Where(item => typeof(IEntity).IsAssignableFrom(item)));

            if (Log.IsTraceEnabled)
                Log.Trace("Load Config datas");
            if (ServerIoc.IsRegisteredWithName<Assembly>("ConfigAssembly"))
                container.Resolve<IConfigReader>().Load(container.ResolveNamed<IDatabaseFactory>("ConfigDbFactory"));

            return container;
        }

        public void InitContainers()
        {
            foreach (var config in GetDistrictConfigs())
            {
                if (_containers.ContainsKey(config.Id))
                    continue;

                var c = LoadDistrictContainer(config);

                if (c == null)
                    continue;

                _containers.Add(config.Id, c);
            }
            //运行期间不使用表刷新
            Settings.AlterTable = false;
        }
        /// <summary> 加载配置列表
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<TDistrictConfig> LoadDistrictConfigs();
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract TDistrictConfig GetDistrictConfig(int id);
        /// <summary>
        /// 创建游戏容器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        public abstract void BuildContainer(ContainerBuilder builder, TDistrictConfig config);

    }
}
