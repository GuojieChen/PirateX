using System;

namespace PirateX.Utils
{
    public static class TimeUtil
    {

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            return GetTimestamp(DateTime.UtcNow);
        }

        public static int GetTimestampAsSecond()
        {
            return (int)(GetTimestamp(DateTime.UtcNow) / 1000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeUtc"></param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime dateTimeUtc)
        {
            return (dateTimeUtc.Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000;
        }
    }
}
