using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PirateX.Domain.Entity;
using PirateX.Domain.Uow;
using ServiceStack.OrmLite;

namespace PirateX.Domain.Repository
{
    public class ServiceStackRepository : IRepository
    {

        private IDbConnection _connection;

        public ServiceStackRepository(IDbConnection connection)
        {
            this._connection = connection;
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : IEntity
        {
            
        }

        public long Insert<TEntity>(TEntity entity, bool identity) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync<TEntity>(TEntity entity) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(object fields, object id)
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(object fields, Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(object id) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<TEntity>(object id) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null; 
        }
    }
}
