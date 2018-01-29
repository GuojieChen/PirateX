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
            Assembly assembly = null; //Assembly.LoadFrom("");

            IGMSDKService service = null;//assembly.GetTypes().FirstOrDefault(item => typeof(IGMSDKService).IsAssignableFrom(item)) as IGMSDKService;

            if(service == null)
                service = new GMSDKService();

            GmsdkService = service;
            // Set MVC DI resolver to use our Autofac container
            DependencyResolver.SetResolver(new AutofacDependencyResolver(GmsdkService.InitContainerBuilder().Build()));
        }
    }
}