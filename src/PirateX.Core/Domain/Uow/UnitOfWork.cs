using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Autofac;
using PirateX.Core.Container.Register;
using PirateX.Core.Domain.Repository;
using StackExchange.Redis;

namespace PirateX.Core.Domain.Uow
{
    public class UnitOfWork : IUnitOfWork
    {

        private IDictionary<string, IRepository> _repoDic = new Dictionary<string, IRepository>();

        private readonly ILifetimeScope _resolver;

        private IDbConnection _dbConnection;
        private IDbTransaction _transaction;
        private IDatabase _redisDatabase;

        private bool _disposed;
        private bool _commited;
        private bool _isTrassactionOpend;

        public UnitOfWork(ILifetimeScope resolver, string name = null)
        {
            this._resolver = resolver;
            if (string.IsNullOrEmpty(name))
            {
                if(_resolver.IsRegistered<IConnectionDistrictConfig>())
                    this._dbConnection = _resolver.Resolve<IDbConnection>();
            }
            else
                this._dbConnection = _resolver.ResolveNamed<IDbConnection>(name);

            if(_resolver.IsRegistered<IDatabase>())
                this._redisDatabase = _resolver.Resolve<IDatabase>();

            _dbConnection?.Open();
        }

        public void BeginTrasaction()
        {
            _transaction = _dbConnection?.BeginTransaction();
            _isTrassactionOpend = true;
        }

        public void BeginTrasaction(IsolationLevel il)
        {
            _transaction = _dbConnection?.BeginTransaction(il);
            _isTrassactionOpend = true;
        }

        public void Commit()
        {
            if (_commited)
                return;
            icommit();
        }

        private void icommit()
        {
            if (!_isTrassactionOpend)
                return;

            try
            {

                _transaction?.Commit();
                _commited = true;
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
        }

        public T Repository<T>() where T : IRepository
        {
            var name = typeof(T).Name;
            if (_repoDic.ContainsKey(name))
                return (T)_repoDic[name];
            else
            {
                var instance = Activator.CreateInstance<T>();
                instance.Resolver = this._resolver;
                instance.DbConnection = _dbConnection;
                instance.DbTransaction = _transaction;
                instance.Redis = _redisDatabase;
                _repoDic.Add(name, instance);

                return instance;
            }
        }

        public void Dispose()
        {
            idispose(true);
            //GC.SuppressFinalize(this);
        }

        private void idispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_transaction != null)
                {
                    if (!_commited)
                        icommit();

                    _transaction?.Dispose();
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
            idispose(false);
        }
    }
}
