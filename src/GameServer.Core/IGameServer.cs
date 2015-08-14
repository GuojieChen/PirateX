using SuperSocket.SocketBase;

namespace GameServer.Core
{
    public interface IGameServer : IAppServer
    {
        void Broadcast<TMessage>(TMessage message, params long[] rids); 
    }
}
