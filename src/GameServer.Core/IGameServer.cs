using System.Collections.Generic;
using System.Linq;
using Autofac;
using GameServer.Container;
using SuperSocket.SocketBase;

namespace GameServer.Core
{
    public interface IGameServer<TGameServerConfig> : IAppServer where TGameServerConfig :IGameServerConfig
    {

        IGameContainer<TGameServerConfig> GameContainer { get; set; }

        /// <summary> 广播消息 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="rids"></param>
        void Broadcast<TMessage>(TMessage message, IQueryable<long> rids); 
    }
}
