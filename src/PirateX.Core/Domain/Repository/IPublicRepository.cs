using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace PirateX.Core.Domain.Repository
{
    public interface IPublicRepository
    {
        ILifetimeScope Resolver { get; set; }

        NamedParameter ConnectionStringName { get; set; }
    }


    public class PublicRepository : IPublicRepository,IDisposable
    {
        public ILifetimeScope Resolver { get; set; }
        public NamedParameter ConnectionStringName { get; set; }

        protected IDisposable DbConnection { get
            {
                var conn = Resolver.Resolve<IDbConnection>(ConnectionStringName);
                conn.Open();
                return conn; 
            }
        }

        public void Dispose()
        {
            Resolver?.Dispose();
        }
    }
}
