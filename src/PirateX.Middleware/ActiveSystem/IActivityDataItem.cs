using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Middleware.ActiveSystem
{
    public interface IActivityDataItem
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ActivityDataItemDescriptonAttribute : Attribute
    {
        public string Name { get; set; }
        public string Des { get; set; }
    }
}
