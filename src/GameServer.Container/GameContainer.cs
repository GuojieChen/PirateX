using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace GameServer.Container
{
    /// <summary> 默认的游戏容器实现
    /// </summary>
    public class GameContainer :IGameContainer
    {
        /// <summary> 配置数据加载器
        /// </summary>
        private readonly IGameServerConfigLoader _serverConfigLoader;

        public GameContainer(IGameServerConfigLoader serverConfigLoader)
        {
            if (serverConfigLoader == null)
                throw new ArgumentNullException(nameof(serverConfigLoader));

            this._serverConfigLoader = serverConfigLoader;
        }
        
        /// <summary>
        /// 本服所负责的游戏ID列表
        /// </summary>
        private IEnumerable<int> ServerIds { get; set; }

        /// <summary> 容器集合
        /// </summary>
        private readonly IDictionary<int, IContainer> _containers = new SortedDictionary<int, IContainer>();

        private readonly object _loadContainerLockHelper = new object();
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

        public IEnumerable<IGameServerConfig> GetServerConfigs()
        {
            var list = _serverConfigLoader.LoadServerConfigs();
            if (ServerIds == null)
                return list; 

            return list.Where(item => ServerIds.Contains(item.Id));
        }

        public IContainer LoadServerContainer(int serverid)
        {
            if (_containers.ContainsKey(serverid))
                _containers.Remove(serverid);

            return LoadServerContainer(_serverConfigLoader.GetServerConfig(serverid));
        }

        public IContainer LoadServerContainer(IGameServerConfig serverConfig)
        {
            if (serverConfig == null)
                return null;

            var builder = new ContainerBuilder();

            builder.Register(c => serverConfig).As<IGameServerConfig>().SingleInstance();

            _serverConfigLoader.SetConfig(builder, serverConfig);

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

    }
}
