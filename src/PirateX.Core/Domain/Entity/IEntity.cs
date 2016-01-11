using System;

namespace PirateX.Core.Domain.Entity
{
    public interface IEntity
    {

    }

    public interface IEntity<TPrimaryKey> : IEntity
    {
        TPrimaryKey Id { get; set; }

        DateTime CreateAt { get; set; }
    }
}
