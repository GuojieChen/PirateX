using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Config
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
