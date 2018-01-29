using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    public class ActivityBasic
    {
        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public int[] Days { get; set; }
    }

    public class GMUIActivityBasicMap : GMUIItemMap<ActivityBasic>
    {
        public GMUIActivityBasicMap()
        {
            Map<GMUITextBoxPropertyMap>(item=>item.StartAt)
                .ToDisplayName("开始时间")
                .ToOrderId(-3);
            Map<GMUITextBoxPropertyMap>(item => item.EndAt)
                .ToDisplayName("结束时间")
                .ToOrderId(-2);
            Map<GMUICheckBoxPropertyMap>(item => item.Days)
                .ToDisplayName("周期")
                .ToOrderId(-1)
                .ToCheckedDataProvider(GMUIDaysListDataProvider.Instance);
        }
    }

    public class GMUIDaysListDataProvider : IGMUICheckedDataProvider
    {
        private GMUIDaysListDataProvider()
        {
        }

        public static GMUIDaysListDataProvider Instance = new GMUIDaysListDataProvider();

        public IEnumerable<GMUICheckedItem> GetCheckedItems()
        {
            return new GMUICheckedItem[] 
            {
                new GMUICheckedItem(){Text = "周一" ,Value = "1",Checked = true},
                new GMUICheckedItem(){Text = "周二" ,Value = "2",Checked = true},
                new GMUICheckedItem(){Text = "周三" ,Value = "3",Checked = true},
                new GMUICheckedItem(){Text = "周四" ,Value = "4",Checked = true},
                new GMUICheckedItem(){Text = "周五" ,Value = "5",Checked = true},
                new GMUICheckedItem(){Text = "周六" ,Value = "6",Checked = true},
                new GMUICheckedItem(){Text = "周七" ,Value = "7",Checked = true},
            };
        }
    }
}
