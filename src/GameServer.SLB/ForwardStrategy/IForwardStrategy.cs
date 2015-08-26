using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.SLB
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
