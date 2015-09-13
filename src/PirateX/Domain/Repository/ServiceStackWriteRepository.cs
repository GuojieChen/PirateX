using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PirateX.Domain.Entity;
using PirateX.Redis.StackExchange.Redis.Ex;
using ServiceStack.OrmLite;
using StackExchange.Redis;

namespace PirateX.Domain.Repository
{
    public class ServiceStackWriteRepository:IWriteRepository
    {
        private IDbConnection _connection;
        private IDatabase _redisDatabase; 

        public ServiceStackWriteRepository(IDbConnection dbConnection)
        {
            _connection = dbConnection;
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : IEntity, new()
        {
            if (entity.GetType().IsAssignableFrom(typeof(IPrivateCacheData)))
            {
                _redisDatabase.SetAsHash(entity);
                //TODO 索引
            }
            else
            {
                _connection.Insert(entity);
            }
        }

        public long Insert<TEntity>(TEntity entity, bool identity) where TEntity : IEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync<TEntity>(TEntity entity) where TEntity : IEntity, new()
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : IEntity, new()
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(object fields, object id)
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(object fields, Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(object id) where TEntity : IEntity, new()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<TEntity>(object id) where TEntity : IEntity, new()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

    }
}
