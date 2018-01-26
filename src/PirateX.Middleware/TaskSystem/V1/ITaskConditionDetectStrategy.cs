

namespace PirateX.Middleware
{
    /// <summary> 任务检测的接口
    /// </summary>
    interface ITaskConditionDetectStrategy
    {
        /// <summary>
        /// 获取辅助条件检测的匹配数据
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="taskconfig"></param>
        /// <returns></returns>
        int GetDetectData(long rid,ITaskConfig taskconfig); 
    }
}
