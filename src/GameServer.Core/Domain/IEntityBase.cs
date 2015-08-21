using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Domain
{
    public interface IEntityBase
    {

    }

    public interface IEntityBase<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }

        DateTime CreateAt { get; set; }
    }
}
