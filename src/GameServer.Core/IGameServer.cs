using System.Collections.Generic;
using System.Linq;
using Autofac;
using GameServer.Core.Cointainer;
using SuperSocket.SocketBase;

namespace GameServer.Core
{
    public interface IGameServer<TGameServerConfig> : IAppServer where TGameServerConfig : IGameServerConfig

    {
        IGameContainer<TGameServerConfig> GameContainer { get; set; }

        IContainer Container { get; }
        /// <summary> 广播消息 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="rids"></param>
        void Broadcast<TMessage>(TMessage message, IQueryable<long> rids); 
    }
}
