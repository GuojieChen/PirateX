using PirateX.Core;

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
