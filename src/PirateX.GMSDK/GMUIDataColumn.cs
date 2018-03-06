using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PirateX.GMSDK
{
    public class GMUIDataColumn
    {
        public string Name { get; set; }
        /// <summary>
        /// 字段显示的名称或者按钮显示的名称
        /// </summary>
        public string DisplayName { get; set; }

        public GMUIHtmlLink Link { get; set; }
    }
    /// <summary>
    /// 超链接方式
    /// </summary>
    public class GMUIHtmlLink
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public string[] Keys { get; set; }
    }
}