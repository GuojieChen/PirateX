using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dapper;
using PirateX.Core.Cache;
using PirateX.Core.Domain.Entity;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using StackExchange.Redis;

namespace PirateX.Core.Domain.Repository
{
    public class RepositoryBase : IRepository,IDisposable
    {
        public ILifetimeScope Resolver { get; set; }
        
        public IRedisSerializer RedisSerializer => Resolver.Resolve<IRedisSerializer>();

        protected IGameCache GameCache => Resolver.Resolve<IGameCache>();

        public void Dispose()
        {
            Resolver?.Dispose();
        }
    }
}
