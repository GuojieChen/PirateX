using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.GMUIListDataProviders
{
    public class GMUIDistrictListDataProvder : IGMUICheckedDataProvider, IGMUIListDataProvider
    {
        public static GMUIDistrictListDataProvder Instance = new GMUIDistrictListDataProvder();

        private GMUIDistrictListDataProvder()
        {

        }

        public IEnumerable<GMUICheckedItem> GetCheckedItems()
        {
            return GMSDKService.Instance.DistrictContainer.GetDistrictConfigs().Select(item => 
            new GMUICheckedItem() { Text=$"{item.Id}-{(item as DistrictConfig).Name}",Value = item.Id.ToString()});
        }

        public IEnumerable<GMUIListItem> GetListItems()
        {
            return GMSDKService.Instance.DistrictContainer.GetDistrictConfigs().Select(item =>
              new GMUICheckedItem() { Text = $"{item.Id}-{(item as DistrictConfig).Name}", Value = item.Id.ToString() });
        }
    }
}
