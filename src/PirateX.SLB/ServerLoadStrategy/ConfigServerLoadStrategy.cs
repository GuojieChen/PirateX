using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PirateX.SLB.ServerLoadStrategy
{
    /// <summary> 配置文件加载策略 
    /// 并且定时刷新配置文件
    /// </summary>
    public class ConfigServerLoadStrategy : IServerLoadStrategy
    {
        private IList<IServerInfo> List { get; set; } 

        public ConfigServerLoadStrategy()
        {
            var serializer = new XmlSerializer(typeof(ServersInfo));

            IList<ServerInfo> list = null;
            List = new List<IServerInfo>();
            /*
            using (StreamWriter writer = new StreamWriter(@"servers.xml"))
            {
                serializer.Serialize(writer, new ServersInfo()
                {
                    Servers = new List<ServerInfo>()
                    {
                        new ServerInfo() {Id = 1,Ip = "127.0.0.1",    Port = 3002,Name = "localhost" },
                        new ServerInfo() {Id = 2,Ip = "192.168.1.212",Port = 3001,Name = "192.168.1.212" }
                    }
                });
            }
            */

            using (var fs = new FileStream("servers.xml", FileMode.Open))
                list = ((ServersInfo)serializer.Deserialize(fs)).Servers;

            int i = 1;
            foreach (var info in list)
            {
                info.Id = i++; 
                List.Add(info);
            }
        }

        public IList<IServerInfo> GetServers()
        {
            

            return List;
        }
    }

    public class ServersInfo
    {
        //[XmlArray(ElementName = "Servers")]
        [XmlArrayItem(ElementName = "Server")]
        public List<ServerInfo> Servers { get; set; } 
    }
}
