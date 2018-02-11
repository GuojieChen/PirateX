using PirateX.GM.App_Start;
using PirateX.GMSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using System.Web.Mvc;

namespace PirateX.GM.Controllers
{
    public class LetterController : Controller
    {
        /// <summary>
        /// 显示历史发放信件
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 发放部分信件 根据昵称或者UID
        /// </summary>
        /// <returns></returns>
        public ActionResult NewToPart()
        {
            ViewBag.DistrictConfigs = AutofacConfig.GmsdkService.DistrictContainer.GetDistrictConfigs();

            return View(); 
        }

        [HttpPost]
        public ActionResult SaveaPart()
        {
            return View();
        }

        /// <summary>
        /// 发放全服信件
        /// </summary>
        /// <returns></returns>
        public ActionResult NewToAll()
        {
            return View();
        }



    }
}