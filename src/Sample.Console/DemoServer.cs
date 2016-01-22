using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using GameServer.Console.SampleConfig;
using GameServer.Console.SampleDomain;
using GameServer.Console.SampleService;
using PirateX;
using PirateX.Core;
using PirateX.Core.Broadcas;
using PirateX.Core.Online;
using PirateX.Core.Service;
using PirateX.Protocol;
using PirateX.ServiceStackV3;

namespace GameServer.Console
{
    public class DemoServer : GameServer<DemoSession,OnlineRole>
    {
        public DemoServer() : base(new DemoServerContainer(), new PokemonXProtocol())
        {
        }

        public override Assembly ConfigAssembly()
        {
            return this.GetType().Assembly;
        }

        public override void IocConfig(ContainerBuilder builder)
        {
            //builder.Register(c => new ProtobufRedisSerializer()).As<IRedisSerializer>().SingleInstance();
            builder.Register(c => new SessionMessageBroadcast<DemoSession>(this)).As<IMessageBroadcast>().SingleInstance();
        }
    }

    public class DistrictConfig : IDistrictConfig
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string ConnectionString { get; set; }
        public string ConfigConnectionString { get; set; }
        public string Redis { get; set; }
        public int RedisDb { get; set; }
        public bool AlterTable { get; set; }
    }

    public class ServerSetting : IServerSetting
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Des { get; set; }
        public string PublicIp { get; set; }
        public string PrivateIp { get; set; }
        public int C { get; set; }
        public string RedisHost { get; set; }
        public bool IsSingle { get; set; }
        public bool AlterTable { get; set; }
        public bool IsMetricOpen { get; set; }
        public List<AppServer> Districts { get; set; }
    }

    public class ContainerSetting : IContainerSetting
    {
        public Assembly ConfigAssembly
        {
            get { return typeof (PetConfig).Assembly; }
        }

        public Assembly EntityAssembly
        {
            get { return typeof (Role).Assembly; }
        }

        public Assembly ServiceAssembly
        {
            get { return typeof (RoleService).Assembly; }
        }
    }

    internal class DemoServerContainer : DistrictContainer
    {
        public DemoServerContainer() : base(new ContainerSetting(), new ServerSetting
        {
            Id = "PirateX.VS-DEV",
            RedisHost = "127.0.1"
        })
        {
            
        }

        private static readonly IEnumerable<DistrictConfig> ServerConfigs = new[]
        {
            new DistrictConfig {
                Id = 1,
                Name = "test 01",
                Redis = "127.0.0.1",
                RedisDb = 1 ,
                ConfigConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                ConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;"},

            new DistrictConfig {
                Id = 2,
                Name = "test 02",
                Redis = "127.0.0.1",
                RedisDb = 2 ,
                ConfigConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                ConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;"},

            new DistrictConfig {
                Id = 3,
                Name = "test 02",
                Redis = "127.0.0.1",
                RedisDb = 3,
                ConfigConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                ConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;"},
        };

        public override IEnumerable<IDistrictConfig> LoadDistrictConfigs()
        {
            return ServerConfigs;
        }

        public override IDistrictConfig GetDistrictConfig(int id)
        {
            return ServerConfigs.FirstOrDefault(item => item.Id == id);
        }

        public override void BuildContainer(ContainerBuilder builder)
        {
        }

        public override IDatabaseFactory GetConfigDatabaseFactory(IDistrictConfig config)
        {
            return new ServiceStackDatabaseFactory(config.ConnectionString);
        }

        public override IDatabaseFactory GetDistrictDatabaseFactory(IDistrictConfig config)
        {
            return new ServiceStackDatabaseFactory(config.ConfigConnectionString);
        }
    }
}