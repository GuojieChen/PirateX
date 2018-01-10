using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Actor
{
    public class RequestDocAttribute:Attribute
    {
        public string Name { get; set; }

        public string Des { get; set; }

        public Type Type { get; set; }
    }
}
