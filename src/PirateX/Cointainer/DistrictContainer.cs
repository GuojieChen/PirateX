using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using PirateX.Config;
using PirateX.Domain.Entity;
using StackExchange.Redis;

namespace PirateX.Cointainer
{
    /// <summary> 默认的游戏容器实现
    /// </summary>
    public abstract class DistrictContainer<TDistrictConfig> :IDistrictContainer<TDistrictConfig> where TDistrictConfig :IDistrictConfig
    {
        /// <summary>
        /// 本服所负责的游戏ID列表
        /// </summary>
        private IEnumerable<int> DistrictIds { get; set; }

        /// <summary> 容器集合
        /// </summary>
        private readonly IDictionary<int, IContainer> _containers = new SortedDictionary<int, IContainer>();

        private readonly object _loadContainerLockHelper = new object();
        public ILifetimeScope ServerIoc { get; set; }

        public IContainer GetDistrictContainer(int districtid)
        {
            if (_containers.ContainsKey(districtid))
                return _containers[districtid];

            lock (_loadContainerLockHelper)
            {
                if (_containers.ContainsKey(districtid))
                    return _containers[districtid];

                var c = LoadDistrictContainer(districtid);
                if (c == null)
                    return null;

                _containers.Add(districtid, c);

                return c;
            }
        }

        public IEnumerable<TDistrictConfig> GetDistrictConfigs()
        {
            var list = LoadDistrictConfigs();
            if (DistrictIds == null)
                return list; 

            return list.Where(item => DistrictIds.Contains(item.Id));
        }

        public IContainer LoadDistrictContainer(int districtid)
        {
            if (_containers.ContainsKey(districtid))
                _containers.Remove(districtid);

            return LoadDistrictContainer(GetDistrictConfig(districtid));
        }

        public IContainer LoadDistrictContainer(TDistrictConfig districtConfig)
        {
            if (districtConfig == null)
                return null;

            var builder = new ContainerBuilder();

            builder.Register(c => districtConfig).As<TDistrictConfig>().SingleInstance();
            builder.Register(c => ConnectionMultiplexer.Connect(districtConfig.Redis)); 

            builder.Register(c => c.Resolve<ConnectionMultiplexer>().GetDatabase(districtConfig.RedisDb)).As<IDatabase>();
            //默认Config缓存数据处理器
            builder.Register(c => new MemoryConfigReader(ServerIoc.ResolveNamed<Assembly>("ConfigAssembly")))
                .As<IConfigReader>()
                .SingleInstance();

            if (ServerIoc.IsRegisteredWithName<Assembly>("EntityAssembly"))
                ServerIoc.Resolve<IDatabaseFactory>().CreateAndAlterTable(ServerIoc.ResolveNamed<Assembly>("EntityAssembly").GetTypes().Where(item=> typeof(IEntity).IsAssignableFrom(item)));

            BuildContainer(builder, districtConfig);

            return builder.Build(); 
        }

        public void InitContainers(params int[] districtids)
        {
            DistrictIds = districtids;

            foreach (var config in GetDistrictConfigs())
            {
                if (_containers.ContainsKey(config.Id))
                    continue;

                var c = LoadDistrictContainer(config);

                if (c == null)
                    continue;

                _containers.Add(config.Id, c);
            }
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
