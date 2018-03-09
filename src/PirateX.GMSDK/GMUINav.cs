using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
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
        public string Method { get; set; }
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
        public List<GMUINav> SubNavs { get; set; } = new List<GMUINav>();
    }
}
