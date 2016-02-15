using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Client.Protocol
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtocolNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public ProtocolNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
