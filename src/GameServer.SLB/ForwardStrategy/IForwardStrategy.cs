using System.Collections.Generic;

namespace GameServer.SLB.ForwardStrategy
{
    /// <summary> 转发策略
    /// </summary>
    public interface IForwardStrategy
    {
        /// <summary> 获取可用的Server
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        IServerInfo GetServerInfo(IList<IServerInfo> servers);
    }
}
