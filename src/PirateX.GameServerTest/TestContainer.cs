using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using PirateX.Core.Container;
using PirateX.Core.Container.Register;
using PirateX.Core.Domain.Entity;
using PirateX.ServiceStackV3;

namespace PirateX.GameServerTest
{
    public class DistrictConfig : IDistrictConfig,IRedisDistrictConfig,IConfigConnectionDistrictConfig,IConnectionDistrictConfig
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
    }
    
    public class TestContainer : DistrictContainer<TestContainer>
    {
        private static readonly IEnumerable<DistrictConfig> ServerConfigs = new DistrictConfig[] 
        {
            new DistrictConfig
            {
                Id = 1,
                Name = "test 01",
                Redis = "127.0.0.1",
                RedisDb = 1,
                ConfigConnectionString =
                    "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                ConnectionString =
                    "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                SecretKey = Guid.NewGuid().ToString()
                
            },

            new DistrictConfig
            {
                Id = 2,
                Name = "test 02",
                Redis = "127.0.0.1",
                RedisDb = 2,
                ConfigConnectionString =
                    "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                ConnectionString =
                    "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                SecretKey = Guid.NewGuid().ToString()
            },

            new DistrictConfig
            {
                Id = 3,
                Name = "test 02",
                Redis = "127.0.0.1",
                RedisDb = 3,
                ConfigConnectionString =
                    "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                ConnectionString =
                    "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;",
                SecretKey = Guid.NewGuid().ToString()
            },
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
            //builder.Register<IDatabaseInitializer>(
            //    c =>
            //        new ServiceStackDatabaseInitializer(
            //            typeof (PetConfig).Assembly.GetTypes().Where(item => typeof (IEntity).IsAssignableFrom(item))))
            //            .SingleInstance();

            builder.Register<IDatabaseInitializer>(c=>new EntityFrameworkDatabaseInitializer());
        }

        public override IDictionary<string, string> GetNamedConnectionStrings()
        {
            return new Dictionary<string, string>()
            {
                {"role", "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;"},
            };
        }
    }
}
