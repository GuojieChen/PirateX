using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Container;

namespace PirateX.Core
{
    public class DefaultServerSetting : IServerSetting
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Des { get; set; }
        public string PublicIp { get; set; }
        public string PrivateIp { get; set; }
        public int C { get; set; }
        public string RedisHost { get; set; }
        public bool IsSingle { get; set; }
        public bool AlterTable { get; set; }
        public bool IsMetricOpen { get; set; }
        public List<AppServer> Districts { get; set; }
    }
}
