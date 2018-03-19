using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Mapping
{
    /// <summary>
    /// 简单的多行文本框
    /// </summary>
    public class GMUITextAreaPropertyMap : GMUIPropertyMap<GMUITextAreaPropertyMap>
    {
        public override string Control => "textarea";
    }
}
