using System;

namespace PirateX.Core.Utils
{
    public static class TimeUtil
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            return DateTime.UtcNow.GetTimestamp();
        }

        public static int GetTimestampAsSecond()
        {
            return (int)(DateTime.UtcNow.GetTimestamp() / 1000);
        }

        public static long GetTimestamp(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000;
        }
    }
}
