using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dapper;
using PirateX.Core.Domain.Entity;
using StackExchange.Redis;

namespace PirateX.Core.Domain.Repository
{
    public class RepositoryBase<T>:IRepository 
        where T : class, IEntity
    {
        public ILifetimeScope Resolver { get; set; }
        public IDbTransaction DbTransaction { get; set; }

        public IDbConnection DbConnection { get; set; }
        public IDatabase Redis { get; set; }

    }
}
