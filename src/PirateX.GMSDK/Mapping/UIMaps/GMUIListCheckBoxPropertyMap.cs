using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Mapping
{
    /// <summary>
    /// 列表式的多选组件
    /// </summary>
    public class GMUIListCheckBoxPropertyMap: GMUIPropertyMap<GMUIListCheckBoxPropertyMap>
    {
        public override string Control => "listcheckbox";
        public IGMUICheckedDataProvider CheckedDataProvider { get; private set; }
        public GMUIListCheckBoxPropertyMap ToCheckedDataProvider(IGMUICheckedDataProvider checkedDataProvider)
        {
            this.CheckedDataProvider = checkedDataProvider;
            return this as GMUIListCheckBoxPropertyMap;
        }
    }
}
