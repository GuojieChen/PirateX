using System.Collections.Generic;

namespace GameServer.SLB.ServerLoadStrategy
{
    /// <summary> 配置文件加载策略 
    /// 并且定时刷新配置文件
    /// </summary>
    public class ConfigServerLoadStrategy : IServerLoadStrategy
    {
        private List<IServerInfo> _list = new List<IServerInfo>()
        {
            new ServerInfo() {Id = 1,Ip = "127.0.0.1",    Port = 3002,Name = "localhost" },
            new ServerInfo() {Id = 2,Ip = "192.168.1.212",Port = 3001,Name = "192.168.1.212" }
        };

        public IList<IServerInfo> GetServers()
        {
            foreach (var info in _list)
                info.Ping = true; 

            return _list;
        }
    }
}
