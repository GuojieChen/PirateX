namespace PirateX.Core.Domain.Entity
{
    /// <summary>
    /// 私有缓存数据
    /// 该数据先被保存到Redis中，再以异步的方式存入到数据库中
    /// </summary>
    public interface IPrivateCacheData
    {

    }
}
