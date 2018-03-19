using System;
using Autofac;

namespace PirateX.Core
{
    public class RepositoryBase : IRepository,IDisposable
    {
        public ILifetimeScope Resolver { get; set; }
        
        public IRedisSerializer RedisSerializer => Resolver.Resolve<IRedisSerializer>();

        protected IGameCache GameCache => Resolver.Resolve<IGameCache>();

        public IConfigReader ConfigReader { get; set; }

        public void Dispose()
        {
            Resolver?.Dispose();
        }
    }
}
