using Newtonsoft.Json;
using PirateX.GMSDK;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
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

        private static Uri remoteUri = new Uri("http://localhost:64692/json.rpc");

        /// <summary>
        /// 获取导航信息
        /// </summary>
        /// <returns></returns>
        public static GMUINav[] GetNavs()
        {
            var client = new JsonRpcClient(remoteUri);
            return client.Invoke<GMUINav[]>("navs");
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
            var client = new JsonRpcClient(remoteUri);
            return client.Invoke<ViewTemplate>($"viewtemplate/{controller}/{action}", queryString.ToString());

            //var file  = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data{Path.DirectorySeparatorChar}{controller}_{action}.json";

            //var json = File.ReadAllText(file);

            //var  template = JsonConvert.DeserializeObject<ViewTemplate>(json);

            //var key = $"{controller}{action}";

            //if (!Templates.ContainsKey(key))
            //    Templates.Add(key,template);

            //return template;
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
            var client = new JsonRpcClient(remoteUri);
            var result = client.Invoke<ViewTemplate>($"viewtemplate/{controller}/{action}", args);

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