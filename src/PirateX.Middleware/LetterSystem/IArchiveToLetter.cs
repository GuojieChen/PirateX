namespace PirateX.Middleware
{
    /// <summary>
    /// 活动归档生成信件
    /// </summary>
    public interface IArchiveToLetter
    {
        /// <summary>
        /// 数据归档，例如排行，
        /// </summary>
        void Archive();

        /// <summary>
        /// 生成信件
        /// </summary>
        /// <param name="rid"></param>
        TLetter Builder<TLetter>(int rid);
    }
}
