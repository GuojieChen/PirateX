namespace PirateX.Core
{
    /// <summary>
    /// 标记数据需要时间戳来标记数据的变化
    /// </summary>
    public interface IEntityTimestamp
    {
    }
    
    public interface IEntityTimestamp<T>:IEntityTimestamp
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        T Timestamp { get; set; }
    }
}
