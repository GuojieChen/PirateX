﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PirateX.GM.App_Start;

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
            ViewBag.ItemMap = AutofacConfig.GmsdkService.GetActivityMaps()
                .FirstOrDefault(item => Equals(item.Name, name));

            return View();
        }
    }
}