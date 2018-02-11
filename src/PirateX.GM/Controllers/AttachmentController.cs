using Newtonsoft.Json;
using PirateX.GM.App_Start;
using PirateX.GMSDK;
using PirateX.GMSDK.Mapping;
using PirateX.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PirateX.GM.Controllers
{
    /// <summary>
    /// 奖励附件管理
    /// </summary>
    public class AttachmentController : Controller
    {
        // GET: Attachment
        public ActionResult Index(int page = 1,int size = 20)
        {
            ViewBag.List = AutofacConfig.GmsdkService.GetGmRepository().GetAttachments(page,size);

            return View();
        }

        /// <summary>
        /// 配置奖励附件
        /// </summary>
        /// <returns></returns>
        public ActionResult New()
        {
            var map = AutofacConfig.GmsdkService.GetRewardItemMap();
            var maps = new List<IGMUIPropertyMap>(map.PropertyMaps);

            var groups = GMUIGroup.ConvertToGMUIGroupList(maps);

            var colclass = "col-md-4 col-sm-6";
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
        public ActionResult Save()
        {
            var map = AutofacConfig.GmsdkService.GetRewardItemMap();

            var builder = new GMUIGroupBuilder(map.PropertyMaps, Request.Form);

            try
            {
                if (string.IsNullOrEmpty(Request.Form["Name"]))
                    builder.Errors.Add("描述不能为空");

                builder.Build();

                if (!builder.Errors.Any())
                {
                    var attachment = new Attachment();
                    var rewardtype = AutofacConfig.GmsdkService.GetRewardType();

                    attachment.Name = Request.Form["Name"];
                    //attachment.Rewards
                    var json = JsonConvert.SerializeObject(builder.Values);
                    attachment.Rewards = (IReward)JsonConvert.DeserializeObject(json, rewardtype);

                    AutofacConfig.GmsdkService.GetGmRepository().AddAttachment(attachment);

                    Session["Activity.New.ShowSuccess"] = true;
                }
                else
                {
                    Session["Activity.New.ShowError"] = true;
                    Session["Activity.New.Errors"] = builder.Errors;
                }
            }
            catch (Exception ex)
            {
                builder.Errors.Add(ex.Message);
                builder.Errors.Add(ex.StackTrace);

                Session["Activity.New.ShowError"] = true;
                Session["Activity.New.Errors"] = builder.Errors;
            }

            return RedirectToAction("Index");
        }
    }
}