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
        public IDbTransaction DbTransaction { get; set; }

        public IDbConnection DbConnection { get; set; }
        public IDatabase Redis { get; set; }

        public IRedisSerializer RedisSerializer => Resolver.Resolve<IRedisSerializer>();

        protected IGameCache GameCache => Resolver.Resolve<IGameCache>();

        public void Dispose()
        {
            Resolver?.Dispose();
            DbTransaction?.Dispose();
            DbConnection?.Dispose();
        }
    }

    public class RepositoryBase<T>: RepositoryBase
        where T : class, IEntity
    {
        
    }
}
