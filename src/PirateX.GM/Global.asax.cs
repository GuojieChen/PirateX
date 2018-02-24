using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using PirateX.GM.App_Start;

namespace PirateX.GM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //检查文件是否有变动
            var dt = Directory.GetLastWriteTime(ConfigurationManager.AppSettings["App_Data_Dir"]);

            if (!_lastWriteTime.HasValue)
            {//第一次
                AutofacConfig.GmsdkService?.SetInstanceNull();
                WorkingCopy.CopySourceToTarget();
                _lastWriteTime = dt;
            }
            else if (dt > _lastWriteTime.Value)
            {//后续 需要重启website
                File.SetLastWriteTimeUtc(Server.MapPath("~/Global.asax"), DateTime.UtcNow);
            }

            AutofacConfig.ConfigureContainer();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private static DateTime? _lastWriteTime;
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }
    }
}
