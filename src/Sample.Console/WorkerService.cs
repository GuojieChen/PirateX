using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using GameServer.Console.Cmd;
using GameServer.Console.SampleConfig;
using GameServer.Console.SampleDomain;
using GameServer.Console.SampleService;
using PirateX;
using PirateX.Core;
using PirateX.Core.Actor;
using PirateX.Core.Broadcas;
using PirateX.Core.Container;
using PirateX.Core.Container.Register;
using PirateX.Core.Container.ServerSettingRegister;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Net;
using PirateX.Protocol;
using PirateX.ServiceStackV3;
using PirateX.Net.SuperSocket;
using PirateX.Protocol.Package;

namespace GameServer.Console
{
    public class WorkerService : ActorService<WorkerService, OnlineRole>
    {
        public override void IocConfig(ContainerBuilder builder)
        {
            //builder.Register(c => new SessionMessageBroadcast<DemoSession>(this)).As<IMessageBroadcast>().SingleInstance();
        }

        public WorkerService() : base(new DemoServerContainer())
        {
        }
    }

    public class DistrictConfig : IDistrictConfig,IRedisDistrictConfig
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Redis { get; set; }
        public int RedisDb { get; set; }
        public string SecretKey { get; set; }
    }

    public class ServerSetting : IServerSetting, IRedisServerSetting
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string RedisHost { get; set; }
        public int RedisDb { get; set; }

    }

    internal class DemoServerContainer : DistrictContainer<DemoServerContainer>
    {
        private static readonly IEnumerable<DistrictConfig> ServerConfigs = new[]
        {
            new DistrictConfig {
                Id = 1,
                Name = "test 01",
                Redis = "192.168.1.34:6379,password=glee1234",
                RedisDb = 11 ,
                },
            new DistrictConfig {
                Id = 2,
                Name = "test 02",
                Redis = "192.168.1.34:6379,password=glee1234",
                RedisDb = 12 ,
                }
        };

        public override IEnumerable<IDistrictConfig> LoadDistrictConfigs()
        {
            return ServerConfigs;
        }

        public override IDistrictConfig GetDistrictConfig(int id)
        {
            return ServerConfigs.FirstOrDefault(item => item.Id == id);
        }

        protected override void BuildDistrictContainer(ContainerBuilder builder)
        {
        }

        protected override void BuildServerContainer(ContainerBuilder builder)
        {
            
        }

        public override IDictionary<string, string> GetNamedConnectionStrings()
        {
            return new Dictionary<string, string>()
            {
                {"role","Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;" },

            };
        }
    }
}