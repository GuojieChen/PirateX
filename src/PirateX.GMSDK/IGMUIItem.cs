using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{

    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public abstract class GMUIItemAttribute:Attribute
    {
        public string DisplayName { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        public string Tips { get; set; }
    }
}
