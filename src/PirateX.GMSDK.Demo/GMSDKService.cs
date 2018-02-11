using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.GMSDK.Demo.ActivityDataItems;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;

namespace PirateX.GMSDK.Demo
{
    public class GMSDKService:IGMSDKService
    {
        public IDistrictContainer DistrictContainer => new DemoDistrictContainer();

        public GMUINav[] GmuiNavs => new GMUINav[]
        {
            new GMUINav()
            {
                ControllerName = "Search",
                DisplayName = "综合查询",
                SubNavs = new GMUINav[]
                {
                    new GMUINav()
                    {
                        ActionName = "search-role",
                        DisplayName = "角色查询",
                    },
                    new GMUINav()
                    {
                        ActionName = "search-activity",
                        DisplayName = "活动查询",
                    },
                }
            },
            new GMUINav()
            {
                ControllerName = "Letter",
                DisplayName = "信件管理",
                SubNavs = new GMUINav[]
                {
                    new GMUINav()
                    {
                        ActionName = "letter-attachment",
                        DisplayName = "附件管理",
                    },
                    new GMUINav()
                    {
                        ActionName = "letter-all",
                        DisplayName = "全服信件",
                    },
                    new GMUINav()
                    {
                        ActionName = "letter-part",
                        DisplayName = "部分信件",
                    },
                }
            },

            new GMUINav()
            {
                ControllerName = "Activity",
                DisplayName = "活动管理",
                SubNavs = new GMUINav[]
                {
                    new GMUINav()
                    {
                        ActionName = "Index",
                        DisplayName = "活动配置",
                    },
                    new GMUINav()
                    {
                        ActionName = "Index",
                        DisplayName = "配置附件",
                    },
                }
            }
        };

        public ContainerBuilder InitContainerBuilder()
        {
            var builder = new ContainerBuilder();

            return builder;
        }

        public IGMUIItemMap[] GetActivityMaps()
        {
            return new IGMUIItemMap[]
            {
                new GMUIDoctorTaskActiveMap(), 
                new GMUILuckyDrawActiveMap(), 
                new GMUITimeCopyActivePropertyMap(),
            };
        }


        private GMRepository _gmRepository = new GMRepository();
        public IGMRepository GetGmRepository()
        {
            return _gmRepository;
        }

        public IGMUIItemMap GetRewardItemMap()
        {
            return new GMUIRewardMap();
        }

        public IActivity GetActivityInstance()
        {
            return new Activity();
        }

        public Type GetRewardType()
        {
            return typeof(Reward);
        }
    }
}
