using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Middleware;

namespace PirateX.GMSDK
{
    public class Attachment
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IReward Rewards { get; set; }
    }

    /// <summary>
    /// 奖励附件
    /// </summary>
    /// <typeparam name="TReward"></typeparam>
    public class Attachment<TReward>: Attachment
        where TReward : IReward
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TReward Rewards { get; set; }
    }
}
