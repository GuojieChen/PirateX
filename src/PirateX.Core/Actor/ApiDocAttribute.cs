using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Actor
{
    public class ApiDocAttribute:Attribute
    {
        public string Des { get; set; }

        public Type BaseType { get; set; }
    }
}
