using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PirateX.Core;
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

        /// <summary>
        /// 活动列表
        /// </summary>
        /// <returns></returns>
        public ActionResult NewList()
        {
            ViewBag.List = AutofacConfig.GmsdkService.GetActivityMaps();
            return View();
        }

        /// <summary>
        /// 显示活动配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult New(string name)
        {
            if (string.IsNullOrEmpty(name))
                return RedirectToAction("NewList");

            var map = AutofacConfig.GmsdkService.GetActivityMaps()
                .FirstOrDefault(item => Equals(item.Name, name));

            var maps = new List<IGMUIPropertyMap>(map.PropertyMaps);
            maps.AddRange(new GMUIActivityBasicMap().PropertyMaps);
            var groups = maps.GroupBy(item => item.GroupName).OrderBy(item => item.Key);

            var colclass = "col-md-4";//默认横向放三个
            if (groups.Count() < 3)//一个card占满一行
            {
                colclass = "col-md-12";
            }

            ViewBag.ItemMap = map;
            ViewBag.Groups = groups;
            ViewBag.ColClass = colclass;

            return View();
        }

        private ActivityBasic ActivityBasicEmpty = new ActivityBasic();

        /// <summary>
        /// 保存活动
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveNew(string name)
        {
            var map = AutofacConfig.GmsdkService.GetActivityMaps()
                .FirstOrDefault(item => Equals(item.Name, name));

            Dictionary<string,string> values = new Dictionary<string, string>();
            try
            {
                if(string.IsNullOrEmpty(Request.Form["Remark"]))
                    throw new Exception("备注不能为空");

                foreach (var propertyMap in map.PropertyMaps)
                {
                    var value = Request.Form[propertyMap.Name];

                    propertyMap?.ValidateAction(value);

                    values.Add(propertyMap.Name, value);
                }

                Session["Activity.New.ShowSuccess"] = true;
            }
            catch (Exception e)
            {
                Session["Activity.New.ShowError"] = true;
                Session["Activity.New.ErrorMsg"] = e.Message;
            }

            var activity = AutofacConfig.GmsdkService.GetActivityInstance();
            activity.StartAt = DateTime.Parse(Request.Form[nameof(ActivityBasicEmpty.StartAt)]);
            activity.EndAt = DateTime.Parse(Request.Form[nameof(ActivityBasicEmpty.EndAt)]);
            activity.Days = Request.Form[nameof(ActivityBasicEmpty.Days)].Split(new char[] { ',' }).Select(int.Parse).ToArray();
            activity.Name = map.Name;
            activity.Remark = Request.Form["Remark"];
            AutofacConfig.GmsdkService.GetGmRepository().AddActivity(activity);

            //保存活动
            return RedirectToAction("New", new { name });
        }

        /// <summary>
        /// 发布活动
        /// </summary>
        /// <returns></returns>
        public ActionResult Publish()
        {
            return View();
        }

        /// <summary>
        /// 配置奖励附件
        /// </summary>
        /// <returns></returns>
        public ActionResult NewAttachment()
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

        [HttpPost]
        public ActionResult SaveAttachment()
        {
            


            return View();
        }
    }
}