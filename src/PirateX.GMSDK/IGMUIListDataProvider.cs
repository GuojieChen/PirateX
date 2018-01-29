using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    /// <summary>
    /// 加载列表数据
    /// </summary>
    public interface IGMUIListDataProvider
    {
        IEnumerable<GMUIListItem> GetListItems();
    }

    public class GMUIListItem
    {
        public string Value { get; set; }

        public string Text { get; set; }
    }
}
