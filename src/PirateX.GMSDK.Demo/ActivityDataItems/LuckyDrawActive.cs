using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;

namespace PirateX.GMSDK.Demo.ActivityDataItems
{
    public class LuckyDrawActive:IActivityDataItem
    {
        public int PetId { get; set; }

        public int FreeCnt { get; set; }
    }

    public class GMUILuckyDrawActiveMap : GMUIItemMap<LuckyDrawActive>
    {
        public GMUILuckyDrawActiveMap()
        {
            base.Name = "幸运转盘活动";
            base.Des = "幸运转盘活动xxxxxxx";

            Map<GMUIDropdownPropertyMap>(item => item.PetId)
                .ToDisplayName("主打精灵")
                .ToTips("注意：必须保证所选精灵在 转盘可配置的精灵表.xlsx 中！")
                .ToListDataProvider(GMUIPetListProvider.Instance);

            Map<GMUITextBoxPropertyMap>(item => item.FreeCnt)
                .ToDisplayName("免费次数")
                .ToTips("xxx");

        }
    }
}
