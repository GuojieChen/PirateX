using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Container;
using PirateX.Middleware.ActiveSystem;

namespace PirateX.GMSDK.Demo
{
    public class GMSDKService:IGMSDKService
    {
        public IDistrictContainer DistrictContainer => new DemoDistrictContainer();

        public GMUINav[] GmuiNavs => new GMUINav[]
        {
            new GMUINav()
            {
                Name = "search",
                DisplayName = "综合查询",
                SubNavs = new GMUINav[]
                {
                    new GMUINav()
                    {
                        Name = "search-role",
                        DisplayName = "角色查询",
                    },
                    new GMUINav()
                    {
                        Name = "search-activity",
                        DisplayName = "活动查询",
                    },
                }
            },
            new GMUINav()
            {
                Name = "letter",
                DisplayName = "信件管理",
                SubNavs = new GMUINav[]
                {
                    new GMUINav()
                    {
                        Name = "letter-attachment",
                        DisplayName = "附件管理",
                    },
                    new GMUINav()
                    {
                        Name = "letter-all",
                        DisplayName = "全服信件",
                    },
                    new GMUINav()
                    {
                        Name = "letter-part",
                        DisplayName = "部分信件",
                    },
                }
            },
        };

        public ContainerBuilder InitContainerBuilder()
        {
            var builder = new ContainerBuilder();

            return builder;
        }

        public Type[] GetActivityDatas()
        {
            return new Type[]
            {
                typeof(DoctorTaskActive),
                typeof(LuckyDrawActive),
            };
        }
    }
}
