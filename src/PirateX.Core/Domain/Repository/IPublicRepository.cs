using System;
using System.Collections.Generic;
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
}
