using System;
using System.Collections.Generic;
using PirateX.Core;

namespace PirateX.Middleware
{
    /// <summary>
    /// 活动模型
    /// </summary>
    public interface IActivity:IEntity<int>,IEntityDistrict
    {
        int Id { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime StartAt { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime EndAt { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        int Type { get; set; }
        /// <summary>
        /// 哪些天开放
        /// </summary>
        int[] Days { get; set; }
        /// <summary>
        /// 活动参数 可以是JSON 或者 JSV的数据结构
        /// </summary>
        string Args { get; set; }
        /// <summary>
        /// 是否已经发放
        /// </summary>
        bool IsSend { get; set; }
        /// <summary>
        /// 活动标记
        /// </summary>
        string Remark { get; set; }
    }
}
