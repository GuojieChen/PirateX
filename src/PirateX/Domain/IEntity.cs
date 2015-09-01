using System;

namespace PirateX.Domain
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
