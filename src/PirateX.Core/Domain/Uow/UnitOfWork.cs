using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autofac;
using PirateX.Core.Domain.Repository;
using StackExchange.Redis;

namespace PirateX.Core.Domain.Uow
{
    public class UnitOfWork :IUnitOfWork
    {

        private IDictionary<string,IRepository> _repoDic = new Dictionary<string, IRepository>();

        private readonly ILifetimeScope _resolver;

        private IDbConnection _dbConnection;
        private IDbTransaction _transaction;
        private IDatabase _redisDatabase;
        private ITransaction _redisTransaction;

        private bool _disposed;
        private bool _commited;



        public UnitOfWork(ILifetimeScope resolver)
        {
            this._resolver = resolver;
            this._dbConnection = _resolver.Resolve<IDbConnection>();
            this._redisDatabase = _resolver.Resolve<IDatabase>();

            _dbConnection.Open();
            _redisTransaction = _redisDatabase.CreateTransaction();
            _transaction = _dbConnection.BeginTransaction();
        }

        public UnitOfWork(ILifetimeScope resolver,IsolationLevel il)
        {
            this._resolver = resolver;
            this._dbConnection = _resolver.Resolve<IDbConnection>();
            this._redisDatabase = _resolver.Resolve<IDatabase>();

            _dbConnection.Open();
            _transaction = _dbConnection.BeginTransaction(il);
            _redisTransaction = _redisDatabase.CreateTransaction();
        }

        public void Commit()
        {
            if (_commited)
                return;
            icommit();
        }

        private void icommit()
        {

            try
            {
                _transaction.Commit();
                _commited = true;
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }

        public T Repository<T>() where T : IRepository
        {
            var name = typeof (T).Name;
            if (_repoDic.ContainsKey(name))
                return (T) _repoDic[name];
            else
            {
                var instance = Activator.CreateInstance<T>();
                instance.DbConnection = _dbConnection;
                instance.RedisTransaction = _redisTransaction;
                instance.DbTransaction = _transaction;
                _repoDic.Add(name,instance);

                return instance;
            }
            
        }

        public void Dispose()
        {
            dispose(true);
            //GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_transaction != null)
                {
                    if(!_commited)
                        icommit();

                    _transaction.Dispose();
                    _transaction = null;
                }

                if (_dbConnection != null)
                {
                    _dbConnection.Close();
                    _dbConnection.Dispose();
                    _dbConnection = null;
                }
                _repoDic.Clear();

            }
            _disposed = true;
        }

        ~UnitOfWork()
        {
            dispose(false);
        }
    }
}
