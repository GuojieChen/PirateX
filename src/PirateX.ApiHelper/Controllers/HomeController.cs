using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PirateX.ApiHelper.App_Start;
using PirateX.ApiHelper.Test;
using PirateX.Core.Actor;

namespace PirateX.ApiHelper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string moduleVersionId)
        {
            ViewBag.ApiGroup = AssemblyContainer.Instance.GetApiGroup(moduleVersionId);
            ViewBag.Groups = AssemblyContainer.Instance.GetGroups();
            return View();
        }

        public ActionResult Details(string modelversionid, string typeguid)
        {
            ViewBag.ApiGrou = AssemblyContainer.Instance.GetApiGroup(modelversionid);
            ViewBag.Details = AssemblyContainer.Instance.GetTypeDetails(AssemblyContainer.Instance.GetRequestType(modelversionid, typeguid));
            ViewBag.Groups = AssemblyContainer.Instance.GetGroups();
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
    }
}