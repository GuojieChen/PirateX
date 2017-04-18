using System.Collections.Generic;

namespace PirateX.Core.Container
{
    public interface IServerSetting
    {
        string Id { get; set; }

        string Name { get; set; }

        /// <summary>
        /// message queue
        /// online
        /// </summary>
        string RedisHost { get; set; } 
    }
}
