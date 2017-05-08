using System;

namespace PirateX.Core.Utils
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
    }
}
