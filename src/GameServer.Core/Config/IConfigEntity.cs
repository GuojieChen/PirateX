﻿namespace GameServer.Core.Config
{
    public interface IConfigEntity
    {
    }

    public interface IConfigEntity<TPrimaryKey> : IConfigEntity
    {
        TPrimaryKey Id { get; set; }
    }
}
