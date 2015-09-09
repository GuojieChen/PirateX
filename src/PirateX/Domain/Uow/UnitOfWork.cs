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

        private IDbConnection dbConnection; 

        public void Commit()
        {
                using (var db = new OrmLiteConnectionFactory("").OpenDbConnection())
                {
                    db.Where<OnlineRole>(item=>item.Did ==2)
                }

        }

        public void QueueCommand(Action<IRepository> command)
        {
            _commands.Add(command);
        }
    }
}
