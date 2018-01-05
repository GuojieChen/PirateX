using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PirateX.ApiHelper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {


            foreach (var file in Directory.GetFiles(Server.MapPath("~/App_Data/")))
            {
                var assembly = Assembly.LoadFrom(file);
                var list = assembly.GetTypes();


            }

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