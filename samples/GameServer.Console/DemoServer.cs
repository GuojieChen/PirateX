using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using GameServer.Core;
using GameServer.Core.Cointainer;
using GameServer.Core.Protocol;
using GameServer.Core.Protocol.V1;
using ServiceStack.Redis;

namespace GameServer.Console
{
    public class DemoServer : PServer<DemoSession,GameServerConfig>
    {
        public DemoServer() : base(new DemoGameContainer(),new PokemonXProtocol())
        {
        }

        public override void SetServerConfig(ContainerBuilder builder)
        {

        }
    }

    public class GameServerConfig : IGameServerConfig
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Db { get; set; }
    }

    class DemoGameContainer : GameContainer<GameServerConfig>
    {
        public DemoGameContainer()
        {

        }

        private static readonly IEnumerable<GameServerConfig> ServerConfigs = new[]
        {
            new GameServerConfig() {Id = 1, Name = "test 01"},
            new GameServerConfig() {Id = 2, Name = "test 02"},
            new GameServerConfig() {Id = 3, Name = "test 03"},
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
            builder.Register(c => new PooledRedisClientManager(new[] { "127.0.0.1" }, new[] { "127.0.0.1" }, config.Db))
                .As<IRedisClientsManager>().SingleInstance();


        }
    }
}
