using Autofac;
using StackExchange.Redis;

namespace PirateX.Core.ServerSettingRegister
{
    [ServerSettingRegister(typeof(RedisServerSettingRegister))]
    public interface IRedisServerSetting
    {
        string RedisHosts { get; set; }

        int RedisDb { get; set; }
    }

    public class RedisServerSettingRegister : IServerSettingRegister
    {
        public void Register(ContainerBuilder builder, IServerSetting setting)
        {
            var redisconfig = setting as IRedisServerSetting;

            if (redisconfig == null)
                return;

            //ConfigurationOptions options = new ConfigurationOptions();
            //foreach (var host in redisconfig.RedisHosts)
            //{
            //    options.EndPoints.Add(host);
            //    options.DefaultDatabase = redisconfig.RedisDb;
            //}

            //Redis连接池  管理全局信息
            builder.Register(c => ConnectionMultiplexer.Connect(redisconfig.RedisHosts))
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

        public void SetUp(IContainer container, IServerSetting setting)
        {
            
        }
    }
}
