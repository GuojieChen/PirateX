using System;
using PirateX.Core.Config;

namespace PirateX.Middleware
{
    public interface ITaskConfig: IConfigIdEntity
    {

    }

    public interface ITaskConditionConfig : IConfigIdEntity
    {
        int TaskId { get; set; }

        short ObjectID { get; set; }
        short Operate { get; set; }
        int V { get; set; }
        string Addition { get; set; }
        bool IsStatic { get; set; }

        string Des { get; set; }
        /// <summary> 掉落概率
        /// </summary>
        int? Rate { get; set; }
    }

    /// <summary>
    /// 战斗任务掉落
    /// </summary>
    [Serializable]
    public class _TaskConditionReward
    {
        public int ConditionId { get; set; }

        public int StoreItemId { get; set; }

        public int Amount { get; set; }

        /// <summary> 相对100 来说的掉落概率
        /// </summary>
        public byte Rate { get; set; }
    }

    /// <summary> 信件任务
    /// </summary>
    [Serializable]
    public class _TaskConditionLetter
    {
        public int Id { get; set; }

        public int ConditionId { get; set; }

        public EnumTaskLetterCategory Category { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Name { get; set; }
    }

    public enum EnumTaskLetterCategory : byte
    {
        TaskStart = 1,
        Visit = 2,
        Win = 3
    }

    public enum EnumPosition : byte
    {
        Random = 0,
        North = 1,
        East = 2,
        South = 3,
        Western = 4,
    }
}
