using System.Collections.Generic;
using Autofac;
using PirateX.Core.Online;
using StackExchange.Redis;

namespace PirateX.Core.Container
{
    public interface IServerSetting
    {
        string Id { get; set; }
        string Name { get; set; }
    }
}
