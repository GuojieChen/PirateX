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
        /// <summary>
        /// 标识
        /// </summary>
        public string Name { get; set; }

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
