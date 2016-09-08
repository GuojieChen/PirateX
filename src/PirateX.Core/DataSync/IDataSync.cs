using System.Collections.Generic;

namespace PirateX.Core.DataSync
{
    public interface IDataSync<T>
    {
        /// <summary> 获取玩家同步数据列表
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="timestamp">客户端最新时间戳</param>
        /// <returns></returns>
        IEnumerable<T> GetList(long rid,long timestamp);
    }
}
