using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using PirateX.Core;
using PirateX.GM.App_Start;
using PirateX.GMSDK;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;

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

            //普通类型
            var groups = GMUIGroup.ConvertToGMUIGroupList(maps);
            var colclass = "col-lg-4 col-md-6 col-sm-12";//默认横向放三个
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

            GMUIGroupBuilder builder = new GMUIGroupBuilder(map.PropertyMaps,Request.Form);


            try
            {

                if (string.IsNullOrEmpty(Request.Form["Remark"]))
                    builder.Errors.Add("备注不能为空");

                builder.Build();

                if (!builder.Errors.Any())
                {
                    var activity = AutofacConfig.GmsdkService.GetActivityInstance();
                    activity.StartAt = DateTime.Parse(Request.Form[nameof(ActivityBasicEmpty.StartAt)]);
                    activity.EndAt = DateTime.Parse(Request.Form[nameof(ActivityBasicEmpty.EndAt)]);
                    activity.Days = Request.Form[nameof(ActivityBasicEmpty.Days)].Split(new char[] { ',' }).Select(int.Parse).ToArray();
                    activity.Name = map.Name;
                    activity.Remark = Request.Form["Remark"];
                    activity.Args = JsonConvert.SerializeObject(builder.Values);
                    AutofacConfig.GmsdkService.GetGmRepository().AddActivity(activity);

                    Session["Activity.New.ShowSuccess"] = true;
                }
                else
                {
                    Session["Activity.New.ShowError"] = true;
                    Session["Activity.New.Errors"] = builder.Errors;
                }
            }
            catch (Exception e)
            {
                builder.Errors.Add(e.Message);
                builder.Errors.Add(e.StackTrace);
                Session["Activity.New.ShowError"] = true;
                Session["Activity.New.Errors"] = builder.Errors;
            }

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

    }
}