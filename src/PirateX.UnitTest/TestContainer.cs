using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PirateX.Core;
using PirateX.Core.Container;
using PirateX.ServiceStackV3;

namespace PirateX.UnitTest
{
    public class TestContainer: DistrictContainer<TestContainer>
    {
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

        public override IDictionary<string, string> GetConnectionStrings()
        {
            return new Dictionary<string, string>();
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
    }
    
}
