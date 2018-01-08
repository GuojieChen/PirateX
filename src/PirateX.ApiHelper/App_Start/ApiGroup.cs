using System;
using System.Collections.Generic;
using System.Reflection;

namespace PirateX.ApiHelper
{
    public class ApiGroup
    {
        public string Name => Assembly.FullName;

        public Assembly Assembly { get; set; }

        public string ModelId => Assembly.ManifestModule.ModuleVersionId.ToString("N");

        public List<Type> Types { get; set; } = new List<Type>();
    }
}