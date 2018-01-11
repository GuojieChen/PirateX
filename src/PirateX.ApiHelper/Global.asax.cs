using System;
using System.Collections.Generic;
using System.Configuration;
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

        private static DateTime? _lastWriteTime; 

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AssemblyContainer.Instance.Load(typeof(ARequest).Assembly);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //检查文件是否有变动
            var dt = Directory.GetLastWriteTime(ConfigurationManager.AppSettings["App_Data_Dir"]);
            if (!_lastWriteTime.HasValue || dt > _lastWriteTime.Value)
            {
                AssemblyContainer.SetInstanceNull();
                CommentsDocContainer.SetInstanceNull();

                WorkingCopy.CopySourceToTarget();

                AssemblyContainer.SetInstanceNull();
                CommentsDocContainer.SetInstanceNull();

                _lastWriteTime = dt;
            }
        }
    }
}
