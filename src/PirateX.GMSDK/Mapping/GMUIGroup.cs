using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Mapping
{
    public class GMUIGroup
    {
        public string DisplayName { get; set; }

        public IEnumerable<IGMUIPropertyMap> Maps { get; set; }

        public bool CanMulti { get; set; }

        public string Id { get; set; }
    }
}
