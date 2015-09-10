using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Domain.Repository;
using PirateX.Online;
using ServiceStack.OrmLite;

namespace PirateX.Domain.Uow
{
    public class UnitOfWork :IUnitOfWork
    {
        private IList<Action<IRepository>> _commands = new List<Action<IRepository>>();

        private IDbConnection _dbConnection;

        public UnitOfWork(IDbConnection dbConnection)
        {
            this._dbConnection = dbConnection;
        }

        public void Commit()
        {
        }

        public void QueueCommand(Action<IRepository> command)
        {
            _commands.Add(command);
        }
    }
}
