using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Mapping
{
    /// <summary>
    /// 单选框
    /// </summary>
    public class GMUICheckBoxPropertyMap : GMUIPropertyMap<GMUICheckBoxPropertyMap>
    {
        public IGMUICheckedDataProvider CheckedDataProvider { get; private set; }
        public GMUICheckBoxPropertyMap ToCheckedDataProvider(IGMUICheckedDataProvider checkedDataProvider)
        {
            this.CheckedDataProvider = checkedDataProvider;
            return this as GMUICheckBoxPropertyMap;
        }
    }
}
