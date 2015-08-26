using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.NLB
{
    public class GameProxyServer:AppServer<GameProxySession,BinaryRequestInfo>
    {
        public GameProxyServer() : base(new GameProxyFilterFactory())
        {
            base.NewRequestReceived += (session, info) =>
            {//通过代理发送给远程server
                session.PushRequestToRemoteServer(info.Body,0,info.Body.Length);
            };


            base.NewSessionConnected += session => session.ConnectRemoteServer();
        }
    }
}
