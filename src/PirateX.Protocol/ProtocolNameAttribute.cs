using System;

namespace PirateX.Protocol
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
