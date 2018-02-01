using System;

namespace PirateX.Core
{

    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class RequestDocAttribute:Attribute
    {
        public string Name { get; set; }

        public string Des { get; set; }

        public Type Type { get; set; }
    }
}
