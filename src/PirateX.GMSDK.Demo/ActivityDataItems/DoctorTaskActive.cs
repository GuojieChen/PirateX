using System;
using System.Collections.Specialized;
using PirateX.GMSDK.Demo.GMUIListDataProviders;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;

namespace PirateX.GMSDK.Demo.ActivityDataItems
{
    public class DoctorTaskActive:IActivityDataItem
    {
        public byte Type { get; set; }

        public int Cnt { get; set; }

        //奖励呢。。
    }

    public class GMUIDoctorTaskActiveMap : GMUIItemMap<DoctorTaskActive>
    {
        public GMUIDoctorTaskActiveMap()
        {
            base.Name = "博士的任务";
            base.Des = "博士的任务xxxxxxx";

            Map<GMUIDropdownPropertyMap>(item => item.Type)
                .ToDisplayName("类型")
                .ToTips("xxxxxx")
                .ToListDataProvider(GMUIDoctoerTypeListProvider.Instance)
                .Validate(s =>
                {
                    if(Equals(s,"0"))
                        throw new Exception("请选择");
                });

            Map<GMUITextBoxPropertyMap>(item=>item.Cnt)
                .ToDisplayName("数量")
                .ToTips("xxxxxx");
        }
    }
}
