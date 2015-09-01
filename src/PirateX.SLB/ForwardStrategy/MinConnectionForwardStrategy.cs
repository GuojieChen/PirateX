using System.Collections.Generic;
using System.Linq;

namespace PirateX.SLB.ForwardStrategy
{
    /// <summary> 最小连接数
    /// </summary>
    public class MinConnectionForwardStrategy:IForwardStrategy
    {
        public IServerInfo GetServerInfo(IList<IServerInfo> servers)
        {
            return servers.Where(item => item.Ping).OrderBy(x => x.ProxyCount).FirstOrDefault();
        }
    }
}
