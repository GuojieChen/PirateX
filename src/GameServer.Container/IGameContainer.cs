﻿using System.Collections.Generic;
using Autofac;

namespace GameServer.Container
{
    public interface IGameContainer
    {
        /// <summary>
        /// 重新加载配置
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        IContainer LoadServerContainer(int serverid);
        /// <summary> 获取管理的配置列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGameServerConfig> GetServerConfigs();
        /// <summary>
        /// 初始化容器信息
        /// </summary>
        /// <param name="servers">启动就初始化的游戏服ID列表</param>
        void InitContainers(params int[] servers);
    }
}
