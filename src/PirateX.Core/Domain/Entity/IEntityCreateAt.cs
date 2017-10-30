using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Domain.Entity
{
    public interface IEntityCreateAt
    {
        long CreateAt { get; set; }

        DateTime CreateAtAsDateTime { get; }
    }
}
