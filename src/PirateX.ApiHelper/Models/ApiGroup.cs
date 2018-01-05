using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PirateX.ApiHelper.Models
{
    public class ApiGroup
    {
        public string Assembly { get; set; }

        public IEnumerable<Type> Types { get; set; }
    }
}