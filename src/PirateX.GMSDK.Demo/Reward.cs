using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.Middleware;

namespace PirateX.GMSDK.Demo
{
    public class Reward:IReward
    {
        public int Coin { get; set; }

        public int Gold { get; set; }
        /// <summary>
        /// 精灵
        /// </summary>
        public int PetId { get; set; }
    }

    public class GMUIRewardMap : GMUIItemMap<Reward>
    {
        public GMUIRewardMap()
        {
            Map<GMUITextBoxPropertyMap>(item => item.Coin)
                .ToDisplayName("钻石");
            Map<GMUITextBoxPropertyMap>(item => item.Gold)
                .ToDisplayName("金币");
            Map<GMUIDropdownPropertyMap>(item => item.PetId)
                .ToDisplayName("主打精灵")
                .ToTips("注意：必须保证所选精灵在 转盘可配置的精灵表.xlsx 中！")
                .ToListDataProvider(GMUIPetListProvider.Instance);
        }
    }
}
