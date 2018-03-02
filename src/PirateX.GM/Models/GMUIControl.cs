using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PirateX.GM.Models
{
    /// <summary>
    /// 控件
    /// </summary>
    public class GMUIControl
    {
        public string Control { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Tips { get; set; }

        public bool IsRequired { get; set; }

        public string DevaultValue { get; set; }

        public int OrderId { get; set; }

        public List<GMUIDataDropdown> Data_DropdownList { get; set; }

        public List<GMUIDataCheckbox> Data_CheckboxList { get; set; }
    }

    public class GMUIControlGroup
    {
        /// <summary>
        /// 有值的情况下标识是一个对象，Controls中的所有属性归该对象
        /// </summary>
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool CanMulti { get; set; }

        public List<GMUIControl> Controls { get; set; }
    }
}