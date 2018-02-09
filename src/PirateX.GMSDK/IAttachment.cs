using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Middleware;

namespace PirateX.GMSDK
{
    /// <summary>
    /// 奖励附件
    /// </summary>
    /// <typeparam name="TReward"></typeparam>
    public interface IAttachment<TReward>
        where TReward : IReward
    {
        int Id { get; set; }

        string Name { get; set; }

        TReward Rewards { get; set; }
    }
}
