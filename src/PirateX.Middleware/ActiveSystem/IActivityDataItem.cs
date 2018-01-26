using System;

namespace PirateX.Middleware
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
