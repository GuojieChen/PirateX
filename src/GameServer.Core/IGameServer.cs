using System.Collections.Generic;
using System.Linq;
using SuperSocket.SocketBase;

namespace GameServer.Core
{
    public interface IGameServer : IGameServer<long>
    {
        
    }

    public interface IGameServer<TRidKey> : IAppServer
    {
        void Broadcast<TMessage>(TMessage message, IQueryable<TRidKey> rids); 
    }
}
