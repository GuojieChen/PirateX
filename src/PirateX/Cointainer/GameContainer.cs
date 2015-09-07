using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using PirateX.Config;
using StackExchange.Redis;

namespace PirateX.Cointainer
{
    /// <summary> 默认的游戏容器实现
    /// </summary>
    public abstract class GameContainer<TGameServerConfig> :IGameContainer<TGameServerConfig> where TGameServerConfig :IGameServerConfig
    {
        /// <summary>
        /// 本服所负责的游戏ID列表
        /// </summary>
        private IEnumerable<int> ServerIds { get; set; }

        /// <summary> 容器集合
        /// </summary>
        private readonly IDictionary<int, IContainer> _containers = new SortedDictionary<int, IContainer>();

        private readonly object _loadContainerLockHelper = new object();
        public ILifetimeScope ServiceContainer { get; set; }

        public IContainer GetServerContainer(int serverid)
        {
            if (_containers.ContainsKey(serverid))
                return _containers[serverid];

            lock (_loadContainerLockHelper)
            {
                if (_containers.ContainsKey(serverid))
                    return _containers[serverid];

                var c = LoadServerContainer(serverid);
                if (c == null)
                    return null;

                _containers.Add(serverid, c);

                return c;
            }
        }

        public IEnumerable<TGameServerConfig> GetServerConfigs()
        {
            var list = LoadServerConfigs();
            if (ServerIds == null)
                return list; 

            return list.Where(item => ServerIds.Contains(item.Id));
        }

        public IContainer LoadServerContainer(int serverid)
        {
            if (_containers.ContainsKey(serverid))
                _containers.Remove(serverid);

            return LoadServerContainer(GetServerConfig(serverid));
        }

        public IContainer LoadServerContainer(TGameServerConfig serverConfig)
        {
            if (serverConfig == null)
                return null;

            var builder = new ContainerBuilder();

            builder.Register(c => serverConfig).As<TGameServerConfig>().SingleInstance();
            builder.Register(c => c.Resolve<ConnectionMultiplexer>().GetDatabase(serverConfig.RedisDb)).As<IDatabase>();
            //默认Config缓存数据处理器
            builder.Register(c => new MemoryConfigReader(ServiceContainer.ResolveNamed<Assembly>("ConfigAssembly")))
                .As<IConfigReader>()
                .SingleInstance();

            SetConfig(builder, serverConfig);

            return builder.Build(); 
        }

        public void InitContainers(params int[] servers)
        {
            ServerIds = servers;

            foreach (var config in GetServerConfigs())
            {
                if (_containers.ContainsKey(config.Id))
                    continue;

                var c = LoadServerContainer(config);

                if (c == null)
                    continue;

                _containers.Add(config.Id, c);
            }
        }


        /// <summary> 加载配置列表
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<TGameServerConfig> LoadServerConfigs();
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract TGameServerConfig GetServerConfig(int id);

        public abstract void SetConfig(ContainerBuilder builder, TGameServerConfig config);

    }
}
