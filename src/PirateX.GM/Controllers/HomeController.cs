using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PirateX.GM.App_Start;
using PirateX.GMSDK;

namespace PirateX.GM.Controllers
{
    /// <summary>
    /// admin template http://carbon.smartisan.io/
    /// </summary>
    public class HomeController : Controller
    {
        private static GMUINav Datas = new GMUINav()
        {
            ControllerName = "Datas",
            DisplayName = "游戏数据",
            SubNavs = new List<GMUINav>()
        };

        private static GMUINav activityNavs = new GMUINav()
        {
            ControllerName = "Activity",
            DisplayName = "活动管理",
            SubNavs = new List<GMUINav>()
                {
                    new GMUINav()
                    {
                        ActionName = "Index",
                        DisplayName = "活动配置",
                    },
                    new GMUINav()
                    {
                        ActionName = "NewList",
                        DisplayName = "添加活动",
                    },
                    new GMUINav()
                    {
                        ActionName = "NewAttachment",
                        DisplayName = "配置奖励",
                    },
                    new GMUINav()
                    {
                        ActionName = "Publish",
                        DisplayName = "活动发布",
                    },
                }
        };

        private static GMUINav attachments = new GMUINav()
        {
            ControllerName = "Attachment",
            DisplayName = "附件管理",
            SubNavs = new List<GMUINav>()
                {
                    new GMUINav()
                    {
                        ActionName = "Index",
                        DisplayName = "查看附件",
                    },
                    new GMUINav()
                    {
                        ActionName = "New",
                        DisplayName = "新建附件",
                    },
                }
        };

        private static GMUINav letterNavs = new GMUINav()
        {
            ControllerName = "Letter",
            DisplayName = "信件管理",
            SubNavs = new List<GMUINav>()
            {
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
        };

        private static GMUINav commonNavs = new GMUINav()
        {
            ControllerName = "CommonForm",
            SubNavs = new List<GMUINav>()
        };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        private static List<GMUINav> Navs = new List<GMUINav>();
        static HomeController()
        {
            foreach (var item in AutofacConfig.GmsdkService.GmuiNavs)
            {
                GMUINav nav = null;

                switch (item.ControllerName)
                {
                    case "Activity": nav = activityNavs; break;
                    case "Attachment": nav = attachments; break;
                    case "Letter": nav = letterNavs; break;
                    case "Datas": nav = Datas; break;
                        //case "CommonForm": nav = commonNavs; break; //特殊，不加到呈现当中
                }

                if (item is GMUICommonFormNav)
                {
                    var cnav = item as GMUICommonFormNav;

                    item.ControllerName = "CommonForm";
                    // cnav.Method = cnav.ActionName;
                    cnav.ActionName = "New/" + cnav.ActionName;
                }
                else if (item is GMUIDatasNav)
                {
                    var cnav = item as GMUIDatasNav;
                    item.ControllerName = "Datas";
                    cnav.ActionName = "Go/" + cnav.ActionName;
                }

                nav?.SubNavs.Add(item);
            }

            Navs.Add(Datas);
            Navs.Add(attachments);
            Navs.Add(activityNavs);
            Navs.Add(letterNavs);
        }

        /// <summary>
        /// 左侧导航
        /// </summary>
        /// <returns></returns>
        public ActionResult Nav()
        {
            ViewBag.Navs = Navs;

            return PartialView();
        }

    }
}