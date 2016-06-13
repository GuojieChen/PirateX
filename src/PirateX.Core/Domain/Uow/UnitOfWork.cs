using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PirateX.Core.Domain.Repository;

namespace PirateX.Core.Domain.Uow
{
    public class UnitOfWork :IUnitOfWork
    {

        private IDictionary<string,IRepository> _repoDic = new Dictionary<string, IRepository>();

        private IDbConnection _dbConnection;
        private IDbTransaction _transaction;
        private bool _disposed;

        public UnitOfWork(IDbConnection dbConnection)
        {
            this._dbConnection = dbConnection;
            _dbConnection.Open();
            _transaction = _dbConnection.BeginTransaction();
        }

        public void Commit()
        {

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
                _repoDic.Clear();

                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
                if (_dbConnection != null)
                {
                    _dbConnection.Dispose();
                    _dbConnection = null;
                }
            }
            _disposed = true;
        }

        ~UnitOfWork()
        {
            dispose(false);
        }
    }
}
