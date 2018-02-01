using System;

namespace PirateX.Core
{
    public class ApiDocAttribute:Attribute
    {
        public string Des { get; set; }

        public Type BaseType { get; set; }
    }
}
