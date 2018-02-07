using System;
using Autofac;

namespace PirateX.Core
{
    public class RepositoryBase : IRepository,IDisposable
    {
        public ILifetimeScope Resolver { get; set; }
        
        public IRedisSerializer RedisSerializer => Resolver.Resolve<IRedisSerializer>();

        protected IGameCache GameCache => Resolver.Resolve<IGameCache>();

        protected IConfigReader ConfigReader => Resolver.Resolve<IConfigReader>();

        public void Dispose()
        {
            Resolver?.Dispose();
        }
    }
}
