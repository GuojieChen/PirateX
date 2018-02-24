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
using PirateX.GMSDK.Demo.Datas;

namespace PirateX.GMSDK.Demo
{
    public class GMSDKService : IGMSDKService
    {
        public IDistrictContainer DistrictContainer => new DemoDistrictContainer();

        private static GMSDKService _instance;
        private static object _lockHelper = new object();
        public static GMSDKService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockHelper)
                    {
                        if (_instance == null)
                        {
                            _instance = new GMSDKService();
                        }
                    }
                }

                return _instance;
            }
        }

        private GMSDKService()
        {

        }

        public GMUINav[] GmuiNavs => new GMUINav[]
        {
            new GMUIDatasNav()
            {
                ControllerName = "Datas",
                ActionName = "Role" ,
                DisplayName = "角色查询" ,
                SearchMap = new GMUIRoleDatasMap(),
                Datas =(s)=>{
                    var table  = new System.Data.DataTable();
                    table.Columns.Add("Id");
                    table.Columns.Add("Name");
                    table.Columns.Add("Lv");
                    table.Columns.Add("UID");

                    for(var i=100;i>0;i--)
                    {
                        var row = table.NewRow();
                        row["Id"] = i;
                        row["Name"] = $"TEST_{i}";
                        row["Lv"] = RandomUtil.Random.Next(1,11);
                        row["UID"] = Guid.NewGuid().ToString();
                        table.Rows.Add(row);
                    }

                    return table;
                }
            },

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
                    s["Rewards"] = JsonConvert.DeserializeObject<Reward>(s["Rewards"].ToString().Replace("\\",""));
                    s["UIDList"] = s["UIDList"].ToString().Split('\r','\n').ToArray();
                    s["NameList"] = s["NameList"].ToString().Split('\r','\n').ToArray();
                    s["TargetDidList"] = s["TargetDidList"].ToString().ToArray<int>();

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

        public void SetInstanceNull()
        {
            _instance = null; 
        }
    }
}
