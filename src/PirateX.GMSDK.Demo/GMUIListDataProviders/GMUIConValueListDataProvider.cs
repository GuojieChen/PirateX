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

        public IEnumerable<GMUIListItem> GetListItems()
        {
            return new GMUIListItem[] 
            {
                new GMUIListItem(){ Text= "草" , Value = "2"},
                new GMUIListItem(){ Text= "电" , Value = "3"},
                new GMUIListItem(){ Text= "水" , Value = "4"},
                new GMUIListItem(){ Text= "岩" , Value = "5"},
                new GMUIListItem(){ Text= "火" , Value = "6"},
            };
        }
    }
}
