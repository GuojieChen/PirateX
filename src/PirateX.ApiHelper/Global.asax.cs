using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PirateX.ApiHelper.App_Start;
using PirateX.ApiHelper.Test;

namespace PirateX.ApiHelper
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AssemblyContainer.Instance.Load(typeof(ARequest).Assembly);

            AssemblyContainer.Instance.Load(Assembly.LoadFile(Server.MapPath("~/App_Data/PokemonIII.Server.Game.dll")));
        }
    }
}
