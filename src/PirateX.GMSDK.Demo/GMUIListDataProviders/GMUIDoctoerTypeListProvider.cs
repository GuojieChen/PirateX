using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.GMUIListDataProviders
{
    public class GMUIDoctoerTypeListProvider : IGMUIListDataProvider
    {
        public IEnumerable<GMUIListItem> GetListItems()
        {
            return new List<GMUIListItem>()
            {
                new GMUIListItem(){Value = "1", Name = "使用面包"},
                new GMUIListItem(){Value = "2", Name = "副本战斗"},
                new GMUIListItem(){Value = "3", Name = "竞技场战斗"},
                new GMUIListItem(){Value = "4", Name = "探宝"},
                new GMUIListItem(){Value = "5", Name = "试炼通关"},
                new GMUIListItem(){Value = "6", Name = "开启宝箱"},
                new GMUIListItem(){Value = "7", Name = "装备重铸"},
                new GMUIListItem(){Value = "8", Name = "招财喵喵"},
            };
        }
    }
}
