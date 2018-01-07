using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PirateX.ApiHelper.Models;
using PirateX.ApiHelper.Test;
using PirateX.Core.Actor;

namespace PirateX.ApiHelper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string name)
        {
            ApiGroup group = null;
            List<string> names = new List<string>();
            //foreach (var file in Directory.GetFiles(Server.MapPath("~/App_Data/")))
            {
                var assembly = typeof(ARequest).Assembly;   //Assembly.ReflectionOnlyLoadFrom(file);
                names.Add(assembly.FullName);

                if (string.IsNullOrEmpty(name))
                {
                    if (group == null)
                        group = GetApiGroup(assembly);
                }
                else if (Equals(assembly.FullName.ToLower(), name.ToLower()))
                {
                    if (group == null)
                        group = GetApiGroup(assembly);
                }
                //else
                //    continue;
            }

            ViewBag.Names = names;
            ViewBag.Group = group;
            return View();
        }

        private ApiGroup GetApiGroup(Assembly assembly)
        {
            var group = new ApiGroup() { Assembly = assembly };

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (typeof(IAction).IsAssignableFrom(type))
                    group.Types.Add(type);
            }

            return group;
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