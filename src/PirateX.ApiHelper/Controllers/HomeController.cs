﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PirateX.ApiHelper.App_Start;
using PirateX.ApiHelper.Test;

namespace PirateX.ApiHelper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string moduleVersionId)
        {
            ViewBag.ApiGroup = AssemblyContainer.Instance.GetApiGroup(moduleVersionId);
            ViewBag.Groups = AssemblyContainer.Instance.GetGroups();
            return View();
        }

        public ActionResult Details(string modelversionid, string typeguid)
        {
            ViewBag.ApiGrou = AssemblyContainer.Instance.GetApiGroup(modelversionid);
            ViewBag.Details = AssemblyContainer.Instance.GetTypeDetails(AssemblyContainer.Instance.GetRequestType(modelversionid, typeguid));
            ViewBag.Groups = AssemblyContainer.Instance.GetGroups();
            return View();
        }

        public ActionResult TypeInfo(string modelid, string id)
        {
            var type = AssemblyContainer.Instance.GetModelType(modelid, id);
            var list = AssemblyContainer.Instance.GetResponseDeses(type);

            ViewBag.Type = type;
            ViewBag.Protomembers = list.Where(item => item.ProtoMember.HasValue).OrderBy(item=>item.ProtoMember.Value);
            //ViewBag.Normal = list.Where(item => !item.ProtoMember.HasValue);

            return View();
        }

        public ActionResult ShowProto()
        {
            var files = Directory.GetFiles(Server.MapPath("~/App_Data/protos"));

            ViewBag.FileNames = files.Select(item => Path.GetFileName(item));

            return View();
        }

        public ActionResult ShowFile(string filename)
        {
            var fileContents = System.IO.File.ReadAllText(Server.MapPath($"~/App_Data/protos/{filename}"));
            ViewBag.FileContents = fileContents;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}