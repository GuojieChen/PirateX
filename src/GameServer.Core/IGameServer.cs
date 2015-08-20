using System.Collections.Generic;
using System.Linq;
using Autofac;
using GameServer.Container;
using SuperSocket.SocketBase;

namespace GameServer.Core
{
    public interface IGameServer : IGameServer<long>
    {
        
    }

    public interface IGameServer<in TRidKey> : IAppServer
    {

        IGameContainer GameContainer { get; set; }

        /// <summary> 广播消息 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="rids"></param>
        void Broadcast<TMessage>(TMessage message, IQueryable<TRidKey> rids); 
    }
}
