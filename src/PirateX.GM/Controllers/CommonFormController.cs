using PirateX.GM.App_Start;
using PirateX.GMSDK;
using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PirateX.GM.Controllers
{
    [RoutePrefix("CommonForm")]
    public class CommonFormController : Controller
    {
        [Route("/CommonForm/New/{id}")]
        public ActionResult New(string id)
        {
            var action = id;

            var nav = AutofacConfig.GmsdkService.GmuiNavs.FirstOrDefault<GMUINav>(item => Equals(action, item.ActionName));

            if(nav !=null)
            {
                var cnav = nav as GMUICommonFormNav;

                var groups = GMUIGroup.ConvertToGMUIGroupList(cnav.Map.PropertyMaps);

                var colclass = "col-md-4 col-sm-6";
                if (groups.Count() < 3)
                {
                    colclass = "col-md-12";
                }

                ViewBag.Action = action;
                ViewBag.ItemMap = cnav.Map ;
                ViewBag.Groups = groups;
                ViewBag.ColClass = colclass;
            }

            return View();
        }

        public ActionResult Save(string id)
        {
            var nav = AutofacConfig.GmsdkService.GmuiNavs.FirstOrDefault<GMUINav>(item => Equals(item.ActionName, id));

            if (nav != null)
            {
                var cnav = nav as GMUICommonFormNav;

                var builder = new GMUIGroupBuilder(cnav.Map.PropertyMaps, Request.Form);
                builder.Build();

                cnav.OnSave?.Invoke(builder.Values);
            }

            return RedirectToAction("New", new { id }); 
        }
    }
}