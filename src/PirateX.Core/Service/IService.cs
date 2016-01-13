using Autofac;

namespace PirateX.Core.Service
{
    public interface IService
    {
        ILifetimeScope Resolver { get; set; }
    }
}
