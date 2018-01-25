using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    public class GMUIItemDropdownAttribute : GMUIItemAttribute
    {
        /// <summary>
        /// IGMUIListDataProvider的实现
        /// </summary>
        public Type ListSourceProvider { get; set; }
    }
}
