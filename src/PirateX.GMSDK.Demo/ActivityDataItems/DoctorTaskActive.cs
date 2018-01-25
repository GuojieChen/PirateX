using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.Middleware.ActiveSystem;

namespace PirateX.GMSDK.Demo
{
    [ActivityDataItemDescripton(Name = "博士的任务",Des = "博士的任务xxxxxxx")]
    public class DoctorTaskActive:IActivityDataItem
    {
        [GMUIItemDropdown(DisplayName = "类型", Tips = "xxx",ListSourceProvider = typeof(GMUIDoctoerTypeListProvider))]
        public byte Type { get; set; }

        [GMUIItemTextBox(DisplayName = "数量",Tips = "xxx")]
        public int Cnt { get; set; }

        //奖励呢。。
    }
}
