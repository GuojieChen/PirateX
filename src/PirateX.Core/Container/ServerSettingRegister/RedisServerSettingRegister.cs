using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core.Session;
using StackExchange.Redis;

namespace PirateX.Core.Container.ServerSettingRegister
{
    [ServerSettingRegister(typeof(RedisServerSettingRegister))]
    public interface IRedisServerSetting
    {
        string RedisHost { get; set; }

        int RedisDb { get; set; }
    }

    public class RedisServerSettingRegister : IServerSettingRegister
    {
        public void Register(ContainerBuilder builder, IServerSetting setting)
        {
            var redisconfig = setting as IRedisServerSetting;

            if (redisconfig == null)
                return;

            //Redis连接池  管理全局信息
            builder.Register(c => ConnectionMultiplexer.Connect(redisconfig.RedisHost))
                .As<ConnectionMultiplexer>()
                .SingleInstance();


            builder.Register(c => c.Resolve<ConnectionMultiplexer>().GetDatabase(redisconfig.RedisDb))
                .As<IDatabase>()
                .InstancePerDependency();

            //在线管理  TODO ,,,,,
            builder.Register(c => new RedisOnlineManager(c.Resolve<ConnectionMultiplexer>()))
                .As<ISessionManager>()
                .SingleInstance();
        }
    }
}
