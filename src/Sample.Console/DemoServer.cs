using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using PirateX;
using PirateX.Cointainer;
using PirateX.Online;
using PirateX.Protocol;
using PirateX.Redis.StackExchange.Redis.Ex;
using ServiceStack.Redis;

namespace GameServer.Console
{
    public class DemoServer : PServer<DemoSession, DistrictConfig,OnlineRole>
    {
        public DemoServer() : base(new DemoDistrictContainer(), new PokemonXProtocol())
        {
        }

        public override Assembly ConfigAssembly()
        {
            return this.GetType().Assembly;
        }

        public override void IocConfig(ContainerBuilder builder)
        {
            //builder.Register(c => new ProtobufRedisSerializer()).As<IRedisSerializer>().SingleInstance();
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
    }

    internal class DemoDistrictContainer : DistrictContainer<DistrictConfig>
    {
        private static readonly IEnumerable<DistrictConfig> ServerConfigs = new[]
        {
            new DistrictConfig {Id = 1, Name = "test 01",Redis = "127.0.0.1",RedisDb = 1},
            new DistrictConfig {Id = 2, Name = "test 02",Redis = "127.0.0.1",RedisDb = 2},
            new DistrictConfig {Id = 3, Name = "test 03",Redis = "127.0.0.1",RedisDb = 3}
        };

        public override IEnumerable<DistrictConfig> LoadDistrictConfigs()
        {
            return ServerConfigs;
        }

        public override DistrictConfig GetDistrictConfig(int id)
        {
            return ServerConfigs.FirstOrDefault(item => item.Id == id);
        }

        public override void BuildContainer(ContainerBuilder builder, DistrictConfig config)
        {
           
        }
    }
}