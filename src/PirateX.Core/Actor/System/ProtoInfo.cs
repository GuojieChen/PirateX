using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace PirateX.Core.Actor.System
{
    public class ProtoInfo : RepAction<ProtoInfoResponse>
    {
        public override string Name => "_protoinfo";

        public override ProtoInfoResponse Play()
        {
            var cname = Context.Request.QueryString["cname"];

            var action = base.ServerReslover.Resolve<IDictionary<string, IAction>>()[cname];

            var attrs = action.GetType().GetCustomAttributes(typeof(ResponseAttribute), false);

            if (attrs.Any())
            {
            }

            return null;
        }


        public static string GetProto<T>()
        {
            return ProtoBuf.Serializer.GetProto<T>();
        }
    }

    public class ProtoInfoResponse
    {
        public string Proto { get; set; }
    }
}
