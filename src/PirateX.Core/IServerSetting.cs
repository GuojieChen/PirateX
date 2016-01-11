using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    public interface IServerSetting
    {
        string Id { get; set; }

        string Name { get; set; }

        string Des { get; set; }

        string PublicIp { get; set; }

        string PrivateIp { get; set; }
        /// <summary>
        /// 类型
        /// 0 默认 gameserver
        /// 1 gm
        /// 2 testtool
        /// 3 debugtool
        /// </summary>
        int C { get; set; }

        /// <summary>
        /// message queue
        /// online
        /// </summary>
        string RedisHost { get; set; } 

        bool IsSingle { get; set; }

        //控制程序是否对管理的服进行表更新，如果不是（默认），则无论服自己是否有开启服更新，都不会进行表扫描和更新
        bool AlterTable { get; set; }

        bool IsMetricOpen { get; set; }

        List<AppServer> Districts { get; set; }

        //TODO 测试服配置？
    }


    public class AppServer
    {
        public int Id { get; set; }

        public string AppSettingId { get; set; }

        public int ServerId { get; set; }
    }
}
