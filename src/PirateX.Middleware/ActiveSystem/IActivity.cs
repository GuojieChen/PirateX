using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware.ActiveSystem
{
    /// <summary>
    /// 活动模型
    /// </summary>
    public interface IActivity:IEnumerable<int>,IEntityDistrict,IEntityCreateAt
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime StartAt { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime EndAt { get; set; }
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
