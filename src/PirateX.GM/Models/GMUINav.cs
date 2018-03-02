using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PirateX.GM.Models
{
    /// <summary>
    /// 导航
    /// </summary>
    public class GMUINav
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; } = "Index";
        /// <summary>
        /// 显示的名称 
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 左侧图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<GMUINav> SubNavs { get; set; }
    }
}