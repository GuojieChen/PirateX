using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PirateX.GM.App_Start;
using PirateX.GMSDK;
using PirateX.GMSDK.Mapping;

namespace PirateX.GM.Controllers
{
    public class ActivityController : Controller
    {

        // GET: Activity
        public ActionResult Index()
        {
            ViewBag.List = AutofacConfig.GmsdkService.GetGmRepository().GetActivities();
            return View();
        }

        public ActionResult NewList()
        {
            ViewBag.List = AutofacConfig.GmsdkService.GetActivityMaps();
            return View();
        }


        public ActionResult New(string name)
        {
            var map = AutofacConfig.GmsdkService.GetActivityMaps()
                .FirstOrDefault(item => Equals(item.Name, name));

            var maps = new List<IGMUIPropertyMap>(map.PropertyMaps);
            maps.AddRange(new GMUIActivityBasicMap().PropertyMaps);
            var groups = maps.GroupBy(item => item.GroupName).OrderBy(item=>item.Key); 
            var colclass = "col-md-4";
            if (groups.Count() < 3)
            {
                colclass = "col-md-12";
            }

            ViewBag.ItemMap = map;
            ViewBag.Groups = groups;
            ViewBag.ColClass = colclass;


            return View();
        }

        public ActionResult Publish()
        {
            return View();
        }

        public ActionResult NewRewards()
        {
            var map = AutofacConfig.GmsdkService.GetRewardItemMap();
            var maps = new List<IGMUIPropertyMap>(map.PropertyMaps);

            var groups = maps.GroupBy(item => item.GroupName).OrderBy(item => item.Key);
            var colclass = "col-md-4";
            if (groups.Count() < 3)
            {
                colclass = "col-md-12";
            }

            ViewBag.ItemMap = map;
            ViewBag.Groups = groups;
            ViewBag.ColClass = colclass;

            return View();
        }
    }
}