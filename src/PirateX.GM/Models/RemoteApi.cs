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
        /// <param name="method"></param>
        /// <returns></returns>
        public static ViewTemplate GetViewTemplate(string method, NameValueCollection queryString)
        {
            var client = new JsonRpcClient(remoteUri);
            var viewtemplate = client.Invoke<ViewTemplate>($"viewtemplate/{method}", queryString.ToString());

            if (!Templates.ContainsKey(method))
                Templates.Add(method, viewtemplate);

            return viewtemplate;
        }

        public static ViewTemplate GetViewTemplateFromLocalStore(string method)
        {
            var key = method;

            if (Templates.ContainsKey(key))
                return Templates[key];

            return null; 
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        public static bool Submit(string method, Dictionary<string, object> args)
        {
            var client = new JsonRpcClient(remoteUri);
            var result = client.Invoke<ViewTemplate>($"submit/{method}", args);

            client.OnError += (e) => 
            {
                Console.WriteLine(e);
            };

            if (result == null)
                return false;

            return true;
        }
    }

    public class SubmitResult
    {
        public bool Success { get; set; }

        public string[] CheckErrors { get; set; }

        public string Msg { get; set; }
    }
}