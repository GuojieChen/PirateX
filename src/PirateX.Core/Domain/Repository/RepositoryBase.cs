using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Dapper;
using PirateX.Core.Domain.Entity;

namespace PirateX.Core.Domain.Repository
{
    public class RepositoryBase<T>:IRepository 
        where T : class, IEntity
    {
        public ILifetimeScope Resolver { get; set; }
        public IDbTransaction DbTransaction { get; set; }

        public IDbConnection DbConnection { get; set; }

        public StackExchange.Redis.ITransaction RedisTransaction { get; set; }

        protected StackExchange.Redis.IDatabase RedisDatabase { get; set; }

        public T GetById(object id)
        {
            //return DbConnection.Get<T>(id,DbTransaction);
            return default(T);
        }

        public void Insert(T t)
        {
            //DbConnection.Insert(t,DbTransaction);
        }

        public void Insert(IEnumerable<T> ts)
        {
            //DbConnection.Insert(ts, DbTransaction);
        }
    }
}
