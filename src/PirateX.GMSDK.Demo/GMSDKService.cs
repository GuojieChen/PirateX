using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.GMSDK.Demo.ActivityDataItems;
using PirateX.GMSDK.Demo.Domain;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;
using Newtonsoft.Json;

namespace PirateX.GMSDK.Demo
{
    public class GMSDKService:IGMSDKService
    {
        public IDistrictContainer DistrictContainer => new DemoDistrictContainer();

        public static GMSDKService Instance = new GMSDKService();

        private GMSDKService()
        {

        }

        public GMUINav[] GmuiNavs => new GMUINav[]
        {
            new GMUICommonFormNav()
            {
                ControllerName = "Letter",
                ActionName = "GuildLetter",
                DisplayName = "公会信件", 
                Map = new GMUIGuildLetterMap()
            },
            new GMUICommonFormNav()
            {
                ControllerName = "Letter",
                ActionName = "LetterToPart",
                DisplayName = "部分信件",
                Map = new GMUISystemLetterMap()
            },
            new GMUICommonFormNav()
            {
                ControllerName = "Letter",
                ActionName = "LetterToAll",
                DisplayName = "全服信件",
                Map = new GMUIAllSystemLetterMap(),
                OnSave =(s)=>
                {
                    var sysletter = JsonConvert.DeserializeObject<SystemLetter>(JsonConvert.SerializeObject(s));

                    Console.WriteLine(sysletter);
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
