using PirateX.GMSDK.Mapping;
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
        /// 左侧图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<GMUINav> SubNavs { get; set; } = new List<GMUINav>();
    }

    /// <summary>
    /// 通用表单
    /// Action作为参数传递
    /// </summary>
    public class GMUICommonFormNav:GMUINav
    {
        /// <summary>
        /// 设置映射的对象
        /// </summary>
        public IGMUIItemMap Map { get; set; }

        public string Method { get; set; }

        /// <summary>
        /// 表单拿到数据
        /// </summary>
        public Action<Dictionary<string,object>> OnSave;
    }
}
