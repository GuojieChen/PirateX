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
            new GMUICommonFormNav()
            {
                ControllerName = "Letter",
                ActionName = "GuildLetter",
                DisplayName = "公会信件", 
                Map = new GMUIGuildLetterMap()
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
