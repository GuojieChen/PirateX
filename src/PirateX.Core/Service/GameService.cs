using Autofac;
using NLog;

namespace PirateX.Core.Service
{
    public class GameService :IService
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public ILifetimeScope Resolver { get; set; }
    }
}
