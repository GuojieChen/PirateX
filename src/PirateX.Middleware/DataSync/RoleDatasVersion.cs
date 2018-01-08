using System;
using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware
{
    public class RoleDatasVersion :IRoleDatasVersion,IEntityTimestamp<byte[]>
    {
        public int Id { get; set; }
        public virtual int Rid { get; set; }
        public virtual byte[] Timestamp { get; set; }
        public DateTime CreateUtcAt { get; set; }
    }
}
