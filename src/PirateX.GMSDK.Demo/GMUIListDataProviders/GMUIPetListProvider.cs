using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.GMUIListDataProviders
{
    public class GMUIPetListProvider : IGMUIListDataProvider
    {
        public static GMUIPetListProvider Instance = new GMUIPetListProvider();

        private GMUIPetListProvider()
        {

        }

        public IEnumerable<GMUIListItem> GetListItems()
        {
            return new List<GMUIListItem>()
            {
                new GMUIListItem(){Value = "0", Text = "请选择"},
                new GMUIListItem(){Value = "1", Text = "妙蛙种子"},
                new GMUIListItem(){Value = "2", Text = "妙蛙草"},
                new GMUIListItem(){Value = "3", Text = "妙蛙花"},
                new GMUIListItem(){Value = "4", Text = "小火龙"},
                new GMUIListItem(){Value = "5", Text = "火恐龙"},
                new GMUIListItem(){Value = "6", Text = "喷火龙"},
                new GMUIListItem(){Value = "7", Text = "杰尼龟"},
                new GMUIListItem(){Value = "8", Text = "卡咪龟"},
            };
        }
    }
}
