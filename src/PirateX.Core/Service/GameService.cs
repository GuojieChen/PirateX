using Autofac;

namespace PirateX.Core.Service
{
    public class GameService :IService
    {
        public ILifetimeScope Container { get; set; }
    }
}
