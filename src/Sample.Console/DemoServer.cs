using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using PirateX;
using PirateX.Cointainer;
using PirateX.Online;
using PirateX.Protocol;
using ServiceStack.Redis;

namespace GameServer.Console
{
    public class DemoServer : PServer<DemoSession, GameServerConfig,OnlineRole>
    {
        public DemoServer() : base(new DemoGameContainer(), new PokemonXProtocol())
        {
        }

        public override Assembly ConfigAssembly()
        {
            return this.GetType().Assembly;
        }

        public override void SetServerConfig(ContainerBuilder builder)
        {
        }
    }

    public class GameServerConfig : IGameServerConfig
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string ConnectionString { get; set; }
        public string ConfigConnectionString { get; set; }
        public string Redis { get; set; }
        public int RedisDb { get; set; }
    }

    internal class DemoGameContainer : GameContainer<GameServerConfig>
    {
        private static readonly IEnumerable<GameServerConfig> ServerConfigs = new[]
        {
            new GameServerConfig {Id = 1, Name = "test 01",Redis = "127.0.0.1",RedisDb = 1},
            new GameServerConfig {Id = 2, Name = "test 02",Redis = "127.0.0.1",RedisDb = 2},
            new GameServerConfig {Id = 3, Name = "test 03",Redis = "127.0.0.1",RedisDb = 3}
        };

        public override IEnumerable<GameServerConfig> LoadServerConfigs()
        {
            return ServerConfigs;
        }

        public override GameServerConfig GetServerConfig(int id)
        {
            return ServerConfigs.FirstOrDefault(item => item.Id == id);
        }

        public override void SetConfig(ContainerBuilder builder, GameServerConfig config)
        {
           
        }
    }
}