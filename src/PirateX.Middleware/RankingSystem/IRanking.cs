using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware.RankingSystem
{
    public interface IRanking:IEntityPrivate,IEntityDistrict
    {
        /// <summary>
        /// 用以排名的积分，
        /// </summary>
        long Score { get; set; }
    }
}
