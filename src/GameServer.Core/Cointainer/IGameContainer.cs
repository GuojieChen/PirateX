using System.Collections.Generic;
using Autofac;

namespace GameServer.Core.Cointainer
{
    public interface IGameContainer<TGameServerConfig> where TGameServerConfig : IGameServerConfig
    {
        /// <summary> 服务器容器
        /// </summary>
        ILifetimeScope ServiceContainer { get; set; }

        /// <summary> 获取游戏服容器
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        IContainer GetServerContainer(int serverid);
        /// <summary>
        /// 重新加载配置
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        IContainer LoadServerContainer(int serverid);
        /// <summary> 获取管理的配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TGameServerConfig> GetServerConfigs();
        /// <summary>
        /// 初始化容器信息
        /// </summary>
        /// <param name="servers">启动就初始化的游戏服ID列表</param>
        void InitContainers(params int[] servers);


        /// <summary> 加载配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TGameServerConfig> LoadServerConfigs();
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TGameServerConfig GetServerConfig(int id);

        void SetConfig(ContainerBuilder builder, TGameServerConfig config);
    }



}
