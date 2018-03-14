using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PirateX.GM.App_Start;
using PirateX.GM.Models;
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

        private static List<GMUINav> Navs = null;
        
        /// <summary>
        /// 左侧导航
        /// </summary>
        /// <returns></returns>
        public ActionResult Nav()
        {
            ViewBag.Navs = RemoteApi.GetNavs();

            return PartialView();
        }

        public ActionResult ViewTemplate(string method)
        {
            var template = RemoteApi.GetViewTemplate(method, Request.QueryString);
            var colclass = "col-lg-4 col-md-6 col-sm-12";//默认横向放三个
            if (template.ControlGroups.Count() < 3)//一个card占满一行
            {
                colclass = "col-md-12";
            }


            var builder = new GMUIGroupBuilder(template.ControlGroups, Request.QueryString);
            builder.Build();

            ViewBag.Method = method;
            ViewBag.ShowForm = template.ControlGroups.Any();
            ViewBag.ColClass = colclass;

            return View(template);
        }

        public ActionResult Submit(string method)
        {
            var template = RemoteApi.GetViewTemplateFromLocalStore(method);

            var builder = new GMUIGroupBuilder(template.ControlGroups, Request.Form);

            builder.Build();

            if (RemoteApi.Submit(method, builder.Values))
                return RedirectToAction("ViewTemplate", "Home", new { method });
            else
                return RedirectToAction("VieTemplate", "Home", new { method });

        }
    }
}