using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Config;
using PirateX.Core.Domain.Uow;
using StackExchange.Redis;
using Topshelf.Logging;

namespace PirateX.Worker.Actions
{
    public abstract class ActionBase:IAction
    {
        public LogWriter Logger { get; set; }
        public string Name { get; set; }
        public WorkerContext Context { get; set; }
        public abstract void Execute();
    }
}
