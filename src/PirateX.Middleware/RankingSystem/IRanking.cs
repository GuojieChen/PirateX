using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware
{
    public interface IRanking:IEntityPrivate,IEntityDistrict
    {
        /// <summary>
        /// 用以排名的积分，
        /// </summary>
        long Score { get; set; }
    }
}
