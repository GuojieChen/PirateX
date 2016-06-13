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
        /// <summary>
        /// 数据版本号，用时间戳来表示
        /// </summary>
        long Vid { get; set; }
    }
}
