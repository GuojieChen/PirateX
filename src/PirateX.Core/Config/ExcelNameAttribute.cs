using System;

namespace PirateX.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcelNameAttribute:Attribute
    {
        public string Name { get; private set; }

        public ExcelNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
