using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware
{
    public interface IArchive:IEntity<long>,IEntityPrivate,IEntityDistrict
    {
        /// <summary>
        /// 是否已经转成信件
        /// </summary>
        bool IsLetterBuild { get; set; }
        /// <summary>
        /// 日期的int表达形式 例如 20180101
        /// </summary>
        int  DateAsint { get; set; }
    }
}
