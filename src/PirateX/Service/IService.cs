using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace PirateX.Service
{
    public interface IService
    {
        ILifetimeScope Container { get; set; }
    }
}
