using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Proxy
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
