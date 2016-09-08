using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware.DataSync
{
    public class RoleDatasVersion :IRoleDatasVersion,IEntityTimestamp<byte[]>
    {
        public virtual DateTime CreateAt { get; set; }
        public long Id { get; set; }
        public virtual long Rid { get; set; }
        public virtual byte[] Timestamp { get; set; }
    }
}
