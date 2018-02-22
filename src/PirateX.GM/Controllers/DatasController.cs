using PirateX.GM.App_Start;
using PirateX.GMSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PirateX.GM.Controllers
{
    public class DatasController : Controller
    {
        public ActionResult Go(string id)
        {
            var nav = AutofacConfig.GmsdkService.GmuiNavs.FirstOrDefault<GMUINav>(item => Equals(item.ActionName, id)) as GMUIDatasNav;

            foreach (var item in nav.SearchMap.PropertyMaps)
            {
                item.DevaultValue = Request.QueryString[item.Name];
            }

            ViewBag.Map = nav.SearchMap;
            ViewBag.Datas = nav.Datas?.Invoke(Request.QueryString);

            return View();
        }
    }
}