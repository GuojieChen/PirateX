using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PirateX.GM.App_Start;

namespace PirateX.GM.Controllers
{
    public class ActivityController : Controller
    {

        // GET: Activity
        public ActionResult Index()
        {
            ViewBag.List = AutofacConfig.GmsdkService.GetActivityDatas();
            return View();
        }

        public ActionResult New(string name)
        {
            var type = AutofacConfig.GmsdkService.GetActivityDatas().FirstOrDefault(item=>Equals(item.Name,name));

            ViewBag.Type = type;

            return View();
        }
    }
}