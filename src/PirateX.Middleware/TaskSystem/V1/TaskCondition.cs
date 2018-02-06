using System;

namespace PirateX.Middleware
{
    public class TaskCondition
    {
        /// <summary> 条件的
        /// </summary>
        public int _ID { get; set; }
        /// <summary> 是否有完成
        /// </summary>
        public bool OK { get; set; }
        /// <summary>
        /// 如果是有关地图点的 表示地图点信息
        /// </summary>
        public TaskMapPoint Point { get; set; }

        public ITaskConditionConfig _Condition { get; set; }

    }

    public class TaskMapPoint
    {
        public short X { get; set; }

        public short Y { get; set; }
    }
}
