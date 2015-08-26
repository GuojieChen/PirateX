using System.Collections.Generic;

namespace GameServer.SLB
{
    /// <summary> 服列表加载策略
    /// </summary>
    public interface IServerLoadStrategy
    {
        /// <summary> 加载服列表
        /// </summary>
        /// <returns></returns>
        IList<IServerInfo> GetServers();
    }
}
