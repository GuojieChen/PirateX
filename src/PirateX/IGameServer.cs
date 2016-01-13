using System.Linq;
using Autofac;
using PirateX.Core;
using SuperSocket.SocketBase;

namespace PirateX
{
    public interface IGameServer : IAppServer

    {
        IServerContainer ServerContainer { get; set; }

        ILifetimeScope Ioc { get; }
        /// <summary> 广播消息 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="rids"></param>
        void Broadcast<TMessage>(TMessage message, IQueryable<long> rids); 
    }
}
