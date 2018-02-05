using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ProtoBuf;

namespace PirateX.Core
{
    /// <summary>
    /// 获取命令列表
    /// </summary>
    public class CommandList : RepAction<string>
    {
        public override string Name => "_CommandList_";
        public override string Play()
        {
            return string.Join(",", base.ServerReslover.Resolve<IDictionary<string, IAction>>().Keys);
        }
    }
    
}
