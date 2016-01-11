using System;
using System.Collections.Generic;
using PirateX.Core.Domain.Repository;

namespace PirateX.Core.Domain.Uow
{
    public class UnitOfWork :IUnitOfWork
    {
        private IList<Action<IWriteRepository>> _commands = new List<Action<IWriteRepository>>();

        private IWriteRepository writeRepository;
        

        public UnitOfWork(IWriteRepository writeRepository)
        {
            this.writeRepository = writeRepository;
        }

        public void Commit()
        {

        }

        public void QueueCommand(Action<IWriteRepository> command)
        {
            _commands.Add(command);
        }
    }
}
