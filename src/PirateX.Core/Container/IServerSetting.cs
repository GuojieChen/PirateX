using System.Collections.Generic;

namespace PirateX.Core.Container
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

        List<AppServer> Districts { get; set; }
    }


    public class AppServer
    {
        public int Id { get; set; }

        public string AppSettingId { get; set; }

        public int ServerId { get; set; }
    }
}
