using System.Collections.Generic;
using Autofac;

namespace GameServer.Container
{
    /// <summary> 配置数据加载器
    /// </summary>
    public interface IGameServerConfigLoader
    {
        /// <summary> 加载配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGameServerConfig> LoadServerConfigs();
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IGameServerConfig GetServerConfig(int id);

        void SetConfig(ContainerBuilder builder, IGameServerConfig config);
    }
}
