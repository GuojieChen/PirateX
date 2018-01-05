using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ProtoBuf;

namespace PirateX.Core.Actor.System
{
    public class ProtoInfo : RepAction<string>
    {
        public override string Name => "_protoinfo";

        public override string Play()
        {
            var cname = Context.Request.QueryString["cname"];

            var action = base.ServerReslover.Resolve<IDictionary<string, IAction>>()[cname];

            var attrs = action.GetType().GetCustomAttributes(typeof(ResponseAttribute), false);

            if (attrs.Any())
            {
                return GetProto((attrs[0] as ResponseAttribute).Type);
            }

            return null; 
        }


        public static string GetProto<T>()
        {
            return ProtoBuf.Serializer.GetProto<T>();
        }

        private static string GetProto(Type type)
        {
            var t = typeof(ProtoBuf.Serializer);

            return Convert.ToString(t.GetMethod("GetProto", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(type)
                .Invoke(t, null));
        }
    }

    [Serializable]
    [ProtoContract(Name = "ProtoInfoResponse")]
    public class ProtoInfoResponse
    {
        [ProtoMember(1)]
        public string Proto { get; set; }
    }
}
