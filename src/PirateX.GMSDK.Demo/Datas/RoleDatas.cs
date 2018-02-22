using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo.Datas
{
    public class RoleDatas
    {
        public int DistrictId { get; set; }

        public string Name { get; set; }

        public string UID { get; set; }
    }

    public class GMUIRoleDatasMap : GMUIItemMap<RoleDatas>
    {
        public GMUIRoleDatasMap()
        {
            Map<GMUIDropdownPropertyMap>(item => item.DistrictId)
                .ToDisplayName("选择服")
                .ToOrderId(-1)
                .ToTips(" ")
                .ToListDataProvider(GMUIDistrictListDataProvder.Instance);

            Map<GMUITextBoxPropertyMap>(item => item.Name)
                .ToDisplayName("昵称")
                .ToTips(" ");

            Map<GMUITextBoxPropertyMap>(item => item.UID)
                .ToDisplayName("UID")
                .ToTips(" ");
        }
    }
}
