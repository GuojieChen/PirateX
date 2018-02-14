using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.GMUIListDataProviders
{
    public class GMUIRewardListDataProvider : IGMUIListDataProvider
    {
        public static GMUIRewardListDataProvider Instance = new GMUIRewardListDataProvider();

        private GMUIRewardListDataProvider()
        {

        }

        public IEnumerable<GMUIListItem> GetListItems()
        {
            var list = GMSDKService.Instance.GetGmRepository().GetAttachments();

            return list.Select(item => new GMUIListItem() {
                Text = item.Id.ToString(),
                 Value = JsonConvert.SerializeObject(item.Rewards)
            });

        }
    }
}
