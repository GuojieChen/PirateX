using System;
using System.Collections.Generic;
using System.Reflection;

namespace PirateX.ApiHelper
{
    public class ApiGroup
    {
        public string GroupName => Assembly.FullName;

        public string Name => $"{Assembly.GetName().Name}({Assembly.GetName().Version})";

        public Assembly Assembly { get; set; }

        public string ModelId => Assembly.ManifestModule.ModuleVersionId.ToString("N");

        public List<Type> Types { get; set; } = new List<Type>();
    }
}