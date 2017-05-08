using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Actor.System
{
    public class ResponseAttribute:Attribute
    {
        public Type Type { get; private set; }
        public ResponseAttribute(Type type)
        {
            Type = type;
        }
    }
}
