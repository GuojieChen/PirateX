using System;
using System.Globalization;

namespace PirateX.Core
{
    public static class TimeUtil
    {
        public static int GetTimestampAsSecond(this DateTime dateTime)
        {
            return (int)(dateTime.GetTimestamp() / 1000);
        }

        public static long GetTimestamp(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000;
        }

        /// <summary>
        /// 时间戳（毫秒级）转化成UTC时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this long timestamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(timestamp).ToLocalTime();
            return dtDateTime;
        }
        /// <summary>
        /// 时间戳（秒级）转化成UTC时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this int timestamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime ToDateTime(this string datetime)
        {
            return DateTime.SpecifyKind(DateTime.Parse(datetime,new CultureInfo("zh-CN")),DateTimeKind.Utc);
        }

        public static string FromDateTime(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
