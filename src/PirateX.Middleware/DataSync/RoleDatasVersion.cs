﻿using System;
using PirateX.Core.Domain.Entity;

namespace PirateX.Middleware
{
    public class RoleDatasVersion :IRoleDatasVersion,IEntityTimestamp<byte[]>
    {
        public long Id { get; set; }
        public virtual long Rid { get; set; }
        public virtual byte[] Timestamp { get; set; }
        public DateTime CreateUtcAt { get; set; }
    }
}
