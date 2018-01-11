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

        public CommentsMember Comments { get; set; }

        public IEnumerable<RequestDocAttribute> RequestDocs { get; set; }

        public IEnumerable<ResponseDes> ResponseDeses { get; set; }

        public string Proto { get; set; }

        public string Json { get; set; }
    }

    public class ResponseDes
    {
        public string ModelId { get; set; }

        public string Name { get; set; }

        public string TypeId { get; set; }

        public string TypeName { get; set; }

        //public ApiDocAttribute PpDoc { get; set; } 
        public CommentsMember Commonts { get; set; }

        public bool IsPrimitive { get; set; }

        /// <summary>
        /// 如果是有ProtoMember标记，则返回序号
        /// </summary>
        public int? ProtoMember { get; set; }
    }
}