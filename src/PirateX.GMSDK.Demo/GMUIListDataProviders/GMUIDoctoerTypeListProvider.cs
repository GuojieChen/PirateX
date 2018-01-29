using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.GMUIListDataProviders
{
    public class GMUIDoctoerTypeListProvider : IGMUIListDataProvider
    {
        public static GMUIDoctoerTypeListProvider Instance = new GMUIDoctoerTypeListProvider();

        private GMUIDoctoerTypeListProvider()
        {

        }

        public IEnumerable<GMUIListItem> GetListItems()
        {
            return new List<GMUIListItem>()
            {
                new GMUIListItem(){Value = "1", Text = "使用面包"},
                new GMUIListItem(){Value = "2", Text = "副本战斗"},
                new GMUIListItem(){Value = "3", Text = "竞技场战斗"},
                new GMUIListItem(){Value = "4", Text = "探宝"},
                new GMUIListItem(){Value = "5", Text = "试炼通关"},
                new GMUIListItem(){Value = "6", Text = "开启宝箱"},
                new GMUIListItem(){Value = "7", Text = "装备重铸"},
                new GMUIListItem(){Value = "8", Text = "招财喵喵"},
            };
        }
    }
}
