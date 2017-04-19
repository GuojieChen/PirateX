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
        protected override OnlineRole CreateOnlineRole(ActorContext context, IToken token)
        {
            return new OnlineRole()
            {
                ClientKeys = context.ClientKeys,
                ServerKeys = context.ServerKeys,
                Did = token.Did,
                Id = token.Rid,
                ResponseConvert = context.ResponseCovnert,
                SessionId = context.SessionId,
                StartUtcAt = DateTime.UtcNow,
                Token = context.Request.Token,
                Uid = token.Uid,
            };
        }

        public override void IocConfig(ContainerBuilder builder)
        {
            //builder.Register(c => new SessionMessageBroadcast<DemoSession>(this)).As<IMessageBroadcast>().SingleInstance();
        }

        public WorkerService():base(new DemoServerContainer())
        {
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
        public string SecretKey { get; set; }
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
        public bool AlterTable { get; set; }
        public bool IsMetricOpen { get; set; }
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
                ConfigConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                ConnectionString = "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;"},

            new DistrictConfig {
                Id = 2,
                Name = "test 02",
                Redis = "192.168.1.34:6379,password=glee1234",
                RedisDb = 12 ,
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

        public override IDictionary<string, string> GetNamedConnectionStrings()
        {
            return new Dictionary<string, string>()
            {
                {"role","Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;" },
                
            };
        }
    }
}