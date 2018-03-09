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

        public GMUIHtmlLink[] Links { get; set; }

        public GMUIRemoveButton[] RemoveButtons { get; set; }
    }


    /// <summary>
    /// 超链接方式
    /// </summary>
    public class GMUIHtmlLink
    {
        public string Method { get; set; }

        public string[] Keys { get; set; }
    }
    /// <summary>
    /// 移除按钮，在列表移除的时候用
    /// </summary>
    public class GMUIRemoveButton
    {
        public string Method { get; set; }

        public string[] Keys { get; set; }
    }
    /// <summary>
    /// 普通按钮
    /// </summary>
    public class GMUIButton
    {
        public string Text { get; set; }

        public string Method { get; set; }
    }
}