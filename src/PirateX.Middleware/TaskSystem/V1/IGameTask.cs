using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware.TaskSystem
{
    public interface IGameTask<TTaskId> : IEntity<int>, IEntityPrivate, IEntityCreateAt
    {
        /// <summary>
        /// 系统任务ID
        /// </summary>
        TTaskId TaskId { get; set; }
        /// <summary>
        /// 是否已完成
        /// </summary>
        bool IsFinished { get; set; }
        /// <summary>
        /// 是否已领取
        /// </summary>
        bool IsReceived { get; set; }

        IList<Int32> StaticConditions { get; set; }
        List<TaskCondition> Conditions { get; set; }
    }

    public interface IGameTask : IGameTask<int>
    {
        
    }

}
