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
        private IList<Action<IReadRepository>> _commands = new List<Action<IReadRepository>>();

        private IDbConnection _dbConnection;

        public UnitOfWork(IDbConnection dbConnection)
        {
            this._dbConnection = dbConnection;
        }

        public void Commit()
        {
        }

        public void QueueCommand(Action<IReadRepository> command)
        {
            _commands.Add(command);
        }
    }
}
