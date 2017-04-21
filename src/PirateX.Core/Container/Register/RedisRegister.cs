using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using StackExchange.Redis;

namespace PirateX.Core.Container.Register
{
    public class RedisRegister:IDistrictConfigRegister
    {
        public void Register(ContainerBuilder builder, IDistrictConfig config)
        {
            var redisConfig = (config as IRedisDistrictConfig);

            if (redisConfig == null)
                return;

            builder.Register(c => ConnectionMultiplexer.Connect(redisConfig.Redis))
                .SingleInstance()
                .AsSelf();

            builder.Register(c => c.Resolve<ConnectionMultiplexer>().GetDatabase(redisConfig.RedisDb))
                .As<IDatabase>()
                .InstancePerDependency();
        }

        public void SetUp(IContainer container, IDistrictConfig config)
        {

        }
    }

    [DistrictConfigRegister(typeof(RedisRegister))]
    public interface IRedisDistrictConfig
    {
        string Redis { get; set; }

        int RedisDb { get; set; }
    }
}
