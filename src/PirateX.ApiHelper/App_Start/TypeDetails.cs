using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PirateX.Core.Actor;
using PirateX.Core.Domain.Entity;

namespace PirateX.ApiHelper.App_Start
{
    public class TypeDetails
    {
        public string Name { get; set; }

        public ApiDocAttribute ApiDoc { get; set; }

        public IEnumerable<RequestDocAttribute> RequestDocs { get; set; }

        public IEnumerable<ResponseDes> ResponseDeses { get; set; }

        public string Proto { get; set; }

        public string Json { get; set; }
    }

    public class ResponseDes
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public string TypeName => Type.Name;

        public bool IsPrimitive => Type.IsPrimitive;

        public ApiDocAttribute PpDoc { get; set; } 
    }
}