using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    /// <summary>
    /// 分布式Id生成器
    /// </summary>
    public static class IdGenerator
    {
        #region id生成器

        private static long BaseTimestamp = new DateTime(2018, 3, 1).GetTimestampAsSecond();

        /// <summary>
        /// 程序标识
        /// </summary>
        public static long AppCode = 2;
        /// <summary>
        /// 时间戳，精确到秒
        /// </summary>
        private static long _preIdTimestamp = 0;
        /// <summary>
        /// 自增标识
        /// </summary>
        private static long _increse;
        /// <summary>
        /// 获取一个基于时间(毫秒)+程序标识+自增值  的唯一id
        /// </summary>
        /// <returns></returns>
        public static long GetPrimaryId()
        {
            var t = DateTime.Now.GetTimestampAsSecond();
            if (t > _preIdTimestamp) //下一秒，重置自增计数
            {
                _preIdTimestamp = t;
                _increse = 0;
            }

            _increse++;

            //32位时间偏移量（以2018-1-1为基准）
            //10位程序标识
            //22位自增值

            //可以跑69年，支持1024个注册程序，每秒 4194304个自增
            return ((_preIdTimestamp-BaseTimestamp) << 32) | AppCode<< 20 | _increse;
        }
        #endregion
    }
}
