using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace PirateX.Core
{
    public class CommandArgs:RepAction<string>
    {
        public override string Name => "_CommandArgs_";

        public override string Play()
        {
            var cmd = Context.Request.QueryString["cmd"];
            var action = base.ServerReslover.Resolve<IDictionary<string, IAction>>()[cmd];

            if (action != null)
            {
                return string.Join(",", action.GetType().GetCustomAttributes<RequestDocAttribute>().Select(item=>item.Name));
            }

            return string.Empty;
        }
    }
}
