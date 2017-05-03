using System;

namespace PirateX.Core.Domain.Entity
{
    public interface IEntity
    {
        DateTime CreateUtcAt { get; set; }
    }

    public interface IEntity<TPrimaryKey> : IEntity
    {
        TPrimaryKey Id { get; set; }
    }
}
