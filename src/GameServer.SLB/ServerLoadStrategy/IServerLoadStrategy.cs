using System.Collections.Generic;

namespace GameServer.SLB.ServerLoadStrategy
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
