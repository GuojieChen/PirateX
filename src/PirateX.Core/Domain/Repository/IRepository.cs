using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using StackExchange.Redis;

namespace PirateX.Core.Domain.Repository
{
    public interface IRepository
    {
        ILifetimeScope Resolver { get; set; }

        IDbTransaction DbTransaction { get; set; }

        IDbConnection DbConnection { get; set; }

        IDatabase Redis { get; set; }
    }
}
