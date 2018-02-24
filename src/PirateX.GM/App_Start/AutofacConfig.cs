using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using PirateX.GMSDK;
using PirateX.GMSDK.Demo;

namespace PirateX.GM.App_Start
{
    public class AutofacConfig
    {

        public static IGMSDKService GmsdkService { get; private set; }

        public static void ConfigureContainer()
        {
            //Assembly assembly = null; //Assembly.LoadFrom("");

            IGMSDKService service = null;
            //var list = new List<Assembly>();
            foreach (var file in Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}App_Data").Where(item => item.EndsWith(".dll")))
            {
                var systemExists = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}bin").Select(Path.GetFileName).ToArray();
                var filename = Path.GetFileName(file);
                if (systemExists.Contains(filename))
                    continue;

                //if (Path.GetFileName(file).StartsWith("PirateX."))
                //    continue;

                var assembly = Assembly.LoadFrom(file);
                //list.Add(assembly);

                var type = assembly.GetTypes().FirstOrDefault(item => typeof(IGMSDKService).IsAssignableFrom(item));
                if (type != null)
                    service = (IGMSDKService)Activator.CreateInstance(type);
            }

            if(service == null)
                service = GMSDKService.Instance;

            GmsdkService = service;
            // Set MVC DI resolver to use our Autofac container
            DependencyResolver.SetResolver(new AutofacDependencyResolver(GmsdkService.InitContainerBuilder().Build()));
        }
    }
}