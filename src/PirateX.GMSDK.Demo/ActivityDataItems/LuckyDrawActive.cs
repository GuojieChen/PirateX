using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.Middleware.ActiveSystem;

namespace PirateX.GMSDK.Demo
{
    [ActivityDataItemDescripton(Name = "幸运转盘活动", Des = "幸运转盘活动xxxxxxx")]
    public class LuckyDrawActive:IActivityDataItem
    {
        [GMUIItemDropdown(DisplayName = "主打精灵", Tips = "注意：必须保证所选精灵在 转盘可配置的精灵表.xlsx 中！", ListSourceProvider = typeof(GMUIPetListProvider))]
        public int PetId { get; set; }

        [GMUIItemTextBox(DisplayName = "免费次数", Tips = "xxx")]
        public int FreeCnt { get; set; }
    }
}
