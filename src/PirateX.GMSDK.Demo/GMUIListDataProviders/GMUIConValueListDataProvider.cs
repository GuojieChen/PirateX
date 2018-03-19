using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.GMUIListDataProviders
{
    public class GMUIConValueListDataProvider : IGMUIListDataProvider
    {
        private GMUIConValueListDataProvider()
        {

        }

        public static GMUIConValueListDataProvider Instance = new GMUIConValueListDataProvider();

        public IEnumerable<GMUIDataDropdown> GetListItems()
        {
            return new GMUIDataDropdown[] 
            {
                new GMUIDataDropdown(){ Text= "草" , Value = "2"},
                new GMUIDataDropdown(){ Text= "电" , Value = "3"},
                new GMUIDataDropdown(){ Text= "水" , Value = "4"},
                new GMUIDataDropdown(){ Text= "岩" , Value = "5"},
                new GMUIDataDropdown(){ Text= "火" , Value = "6"},
            };
        }
    }
}
