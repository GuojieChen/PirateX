﻿using System.Web;
using System.Web.Optimization;

namespace PirateX.GM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/popper.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/chart.min.js",
                      "~/Scripts/carbon.js",
                      "~/Scripts/demo.js"
                          //"~/Scripts/respond.js"
                          ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/simple-line-icons/css/simple-line-icons.css",
                "~/Content/font-awesome/css/fontawesome-all.min.css",
                      //"~/Content/bootstrap.css",
                      "~/Content/styles.css"));
        }
    }
}
