using System.Collections.Generic;
using System.Linq;

namespace PirateX.SLB.ForwardStrategy
{
    /// <summary> 轮询策略
    /// </summary>
    public class PollingForwardStrategy : IForwardStrategy
    {
        private int _preIndex; 

        public IServerInfo GetServerInfo(IList<IServerInfo> servers)
        {
            if (!servers.Any())
                return null;

            if (_preIndex >= servers.Count)
            {
                _preIndex = 0;
                return servers[0];
            }
            else
            {
                return servers[_preIndex++]; 
            } 
        }
    }
}
