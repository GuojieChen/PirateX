using Autofac;

namespace PirateX.Core.Service
{
    public class GameService :IService
    {
        public ILifetimeScope Resolver { get; set; }
    }
}
