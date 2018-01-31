using Autofac;

namespace PirateX.Core
{
    public interface IRepository
    {
        ILifetimeScope Resolver { get; set; }
    }
}
