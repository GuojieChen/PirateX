using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace PirateX.GM.Models
{
    /// <summary>
    /// 同业务端进行交互
    /// </summary>
    public class RemoteApi
    {
        private static string RemoteUri = "";

        private static string SecretKey = "";



        /// <summary>
        /// 获取导航信息
        /// </summary>
        /// <returns></returns>
        public static List<GMUINav> GetNavs()
        {
            return new List<GMUINav>()
            {
                new GMUINav()
                {
                    ControllerName = "Search",
                    DisplayName = "综合查询",
                    SubNavs = new List<GMUINav>()
                        {
                            new GMUINav()
                            {
                                ActionName = "rolesearch",
                                DisplayName = "角色查询",
                            },
                        }
                },
                new GMUINav()
                {
                    ControllerName = "Activity",
                    DisplayName = "活动管理",
                    SubNavs = new List<GMUINav>()
                        {
                            new GMUINav()
                            {
                                ActionName = "Index",
                                DisplayName = "活动配置",
                            },
                            new GMUINav()
                            {
                                ActionName = "NewList",
                                DisplayName = "添加活动",
                            },
                            new GMUINav()
                            {
                                ActionName = "NewAttachment",
                                DisplayName = "配置奖励",
                            },
                            new GMUINav()
                            {
                                ActionName = "Publish",
                                DisplayName = "活动发布",
                            },
                        }
                },

                new GMUINav()
                {
                    ControllerName = "Attachment",
                    DisplayName = "附件管理",
                    SubNavs = new List<GMUINav>()
                        {
                            new GMUINav()
                            {
                                ActionName = "Index",
                                DisplayName = "查看附件",
                            },
                            new GMUINav()
                            {
                                ActionName = "New",
                                DisplayName = "新建附件",
                            },
                        }
                },
                new GMUINav()
                {
                    ControllerName = "Letter",
                    DisplayName = "信件管理",
                    SubNavs = new List<GMUINav>()
                    {
                            new GMUINav()
                            {
                                ActionName = "letter-all",
                                DisplayName = "全服信件",
                            },
                            new GMUINav()
                            {
                                ActionName = "letter-part",
                                DisplayName = "部分信件",
                            },
                    }
                },
                new GMUINav()
                {
                    ControllerName = "Letter",
                    DisplayName = "信件管理",
                    SubNavs = new List<GMUINav>()
                    {
                            new GMUINav()
                            {
                                ActionName = "letter-all",
                                DisplayName = "全服信件",
                            },
                            new GMUINav()
                            {
                                ActionName = "letter-part",
                                DisplayName = "部分信件",
                            },
                    }
                }
            };
        }

        private static Dictionary<string, ViewTemplate> Templates = new Dictionary<string, ViewTemplate>();

        /// <summary>
        /// 获取页面模板
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ViewTemplate GetViewTemplate(string controller, string action, NameValueCollection queryString)
        {
            var file  = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data{Path.DirectorySeparatorChar}{controller}_{action}.json";

            var json = File.ReadAllText(file);

            var  template = JsonConvert.DeserializeObject<ViewTemplate>(json);

            var key = $"{controller}{action}";

            if (!Templates.ContainsKey(key))
                Templates.Add(key,template);

            return template;
        }

        public static ViewTemplate GetViewTemplateFromLocalStore(string controller, string action)
        {
            var key = $"{controller}{action}";

            if (Templates.ContainsKey(key))
                return Templates[key];

            return null; 
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="args"></param>
        public static SubmitResult Submit(string controller, string action, Dictionary<string, object> args)
        {
            return new SubmitResult() { Success = true };
        }
    }

    public class SubmitResult
    {
        public bool Success { get; set; }

        public string[] CheckErrors { get; set; }

        public string Msg { get; set; }
    }
}