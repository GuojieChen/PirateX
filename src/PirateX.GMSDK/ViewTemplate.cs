using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PirateX.GMSDK
{
    /// <summary>
    /// 页面模板
    /// </summary>
    public class ViewTemplate
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string Title { get; set; }

        public string SubmitBottenText { get; set; }

        /// <summary>
        /// 控件列表
        /// </summary>
        public GMUIControlGroup[] ControlGroups { get; set; }

        public string FormMethod { get; set; }

        /// <summary>
        /// 数据头
        /// </summary>
        public GMUIDataColumn[] Columns { get; set; }
        /// <summary>
        /// 数据体
        /// </summary>
        public Dictionary<string,object>[] Rows {get; set; }
        /// <summary>
        /// 数据总共的页数
        /// </summary>
        public int TotalPageCount { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }
    }
}