using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using PirateX.ApiHelper.App_Start;

namespace PirateX.ApiHelper
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AssemblyContainer.Instance.Load(typeof(ARequest).Assembly);

            var list = new List<Assembly>();
            foreach (var file in Directory.GetFiles(Server.MapPath("~/App_Data")).Where(item=>item.EndsWith(".dll")))
            {
                list.Add(Assembly.LoadFrom(file));
            }

            CommentsDocContainer.Instance.Load(Directory.GetFiles(Server.MapPath("~/App_Data")).Where(item => item.EndsWith(".xml")).ToArray());
            AssemblyContainer.Instance.Load(list.ToArray());
        }
    }
}
