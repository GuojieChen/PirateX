using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Topshelf.Logging;

namespace PirateX.Worker.Actions
{
    public interface IAction
    {
        LogWriter Logger { get; set; }

        /// <summary>
        /// 动作名称
        /// </summary>
        string Name { get; set; }

        WorkerContext Context { get; set; }

        void Execute();
    }
}
