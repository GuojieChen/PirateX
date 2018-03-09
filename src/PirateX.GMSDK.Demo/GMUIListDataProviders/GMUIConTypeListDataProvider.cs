using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.GMUIListDataProviders
{
    public class GMUIConTypeListDataProvider : IGMUIListDataProvider
    {
        private GMUIConTypeListDataProvider()
        {

        }

        public static GMUIConTypeListDataProvider Instance = new GMUIConTypeListDataProvider();

        public IEnumerable<GMUIDataDropdown> GetListItems()
        {
            return new GMUIDataDropdown[] 
            {
                new GMUIDataDropdown(){ Text = "要求精灵属性" ,Value = "1" },
                new GMUIDataDropdown(){ Text = "要求精灵职业" ,Value = "2" },
            };
        }
    }
}
