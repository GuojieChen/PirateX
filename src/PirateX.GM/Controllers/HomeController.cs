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
        private static GMUINav activityNavs = new GMUINav()
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
            SubNavs = new GMUINav[]
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
            SubNavs = new GMUINav[]
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

        /// <summary>
        /// 左侧导航
        /// </summary>
        /// <returns></returns>
        public ActionResult Nav()
        {
            List<GMUINav> navs = new List<GMUINav>();

            navs.Add(attachments);
            navs.Add(activityNavs);
            navs.Add(letterNavs);

            ViewBag.Navs = navs;//AutofacConfig.GmsdkService.GmuiNavs;

            return PartialView();
        }

    }
}