using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PirateX.Core.Container;
using PirateX.GM.App_Start;
using PirateX.GMSDK;

namespace PirateX.GM.Controllers
{
    /// <summary>
    /// admin template http://carbon.smartisan.io/
    /// </summary>
    public class HomeController : Controller
    {
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
            ViewBag.Navs = AutofacConfig.GmsdkService.GmuiNavs;

            return PartialView();
        }
    }
}