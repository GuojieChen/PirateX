using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PirateX.ApiHelper.Models
{
    public class ApiGroup
    {
        public Assembly Assembly { get; set; }

        public List<Type> Types { get; set; } = new List<Type>();
    }
}