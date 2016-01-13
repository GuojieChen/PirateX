using Autofac;

namespace PirateX.Core.Service
{
    public interface IService
    {
        ILifetimeScope Container { get; set; }
    }
}
