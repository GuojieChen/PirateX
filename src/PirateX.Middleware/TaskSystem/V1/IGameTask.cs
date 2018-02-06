using System;
using System.Collections.Generic;
using PirateX.Core;

namespace PirateX.Middleware
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

        IList<int> StaticConditions { get; set; }
        List<TaskCondition> Conditions { get; set; }
    }

    public interface IGameTask : IGameTask<int>
    {
        
    }

}
