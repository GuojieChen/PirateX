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
    public class ServiceStackReadRepository : IReadRepository
    {



        public void Dispose()
        {
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            throw new NotImplementedException();
        }
    }
}
