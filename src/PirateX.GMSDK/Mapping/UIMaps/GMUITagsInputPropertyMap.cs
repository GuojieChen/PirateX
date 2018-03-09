using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Mapping
{
    /// <summary>
    /// 适用于多选
    /// </summary>
    //https://bootstrap-tagsinput.github.io/bootstrap-tagsinput/examples/
    public class GMUITagsInputPropertyMap: GMUIPropertyMap<GMUITagsInputPropertyMap>
    {
        public override string Control => "tagsinput";

    }
}
