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
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Core.Session;
using PirateX.Net;
using PirateX.Protocol;
using PirateX.ServiceStackV3;
using PirateX.Net.SuperSocket;
using PirateX.Protocol.Package;

namespace GameServer.Console
{
    public class WorkerService : ActorService<WorkerService>
    {
        protected override string DefaultResponseCovnert => "json";

        public WorkerService() : base(new DemoServerContainer())
        {
        }

        protected override bool VerifyToken(IDistrictConfig config, IToken token)
        {
            return true;
        }
    }

    public class DistrictConfig : IDistrictConfig
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string SecretKey { get; set; }
        public int TargetId { get; set; }
        //public string Redis { get; set; }
        //public int RedisDb { get; set; }
    }

    public class ServerSetting : IServerSetting
        //,IRedisServerSetting
    {
        public string Id { get; set; }
        public string Name { get; set; }

        //public string RedisHosts { get; set; }
        //public int RedisDb { get; set; }

    }

    internal class DemoServerContainer : DistrictContainer<DemoServerContainer>
    {
        private static readonly IEnumerable<DistrictConfig> ServerConfigs = new[]
        {
            new DistrictConfig {
                Id = 1,
                Name = "test 01",
                //Redis = "127.0.0.1:6379,password=glee1234",
                //RedisDb = 1,
                },
            new DistrictConfig {
                Id = 2,
                Name = "test 02",
                //Redis = "127.0.0.1:6379,password=glee1234",
                //RedisDb = 2,
                }
        };


        public override IEnumerable<IDistrictConfig> GetDistrictConfigs()
        {
            return ServerConfigs;
        }

        public override IDistrictConfig GetDistrictConfig(int id)
        {
            return ServerConfigs.FirstOrDefault(item => item.Id == id);
        }
        
#if DEBUG
        public override IServerSetting GetServerSetting()
        {
            return base.GetDefaultSeting<ServerSetting>();
        }
#endif


        protected override void BuildDistrictContainer(ContainerBuilder builder)
        {
        }

        protected override void BuildServerContainer(ContainerBuilder builder)
        {
            builder.Register(c => new MemorySessionManager())
                .As<ISessionManager>()
                .SingleInstance();

        }

        public override IDictionary<string, string> GetNamedConnectionStrings()
        {
            return new Dictionary<string, string>()
            {
                //{"role","role_123" },
                //{"cps","role_456" },
            };
        }
    }
}