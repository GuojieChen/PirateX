using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    /// <summary>
    /// 导航
    /// </summary>
    public class GMUINav
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; } = "Index" ; 

        /// <summary>
        /// 显示的名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public GMUINav[] SubNavs { get; set; }
    }
}
