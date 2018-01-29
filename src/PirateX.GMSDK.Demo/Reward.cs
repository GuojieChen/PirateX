using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo
{
    public class Reward
    {
        public int Coin { get; set; }

        public int Gold { get; set; }

    }

    public class GMUIRewardMap : GMUIItemMap<Reward>
    {
        public GMUIRewardMap()
        {
            Map<GMUITextBoxPropertyMap>(item => item.Coin)
                .ToDisplayName("钻石");
            Map<GMUITextBoxPropertyMap>(item => item.Gold)
                .ToDisplayName("金币");
        }
    }
}
