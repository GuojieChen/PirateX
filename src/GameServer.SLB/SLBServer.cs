using System;
using GameServer.SLB.ForwardStrategy;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.SLB
{
    public class SLBServer:AppServer<SLBSession,BinaryRequestInfo>
    {
        /// <summary> 服列表加载策略 
        /// </summary>
        public IServerLoadStrategy  ServerLoadStrategy { get; set; }

        public IForwardStrategy ForwardStrategy { get; set; }

        
        public SLBServer() : base(new SLBFilterFactory())
        {
            //创建远程代理
            base.NewSessionConnected += session =>
            {
                if (session.CreateNewSessionHandler == null)
                    session.CreateNewSessionHandler += CreateNewSessionHandler;

                session.ConnectProxyServer();
            };
            //将代理收到的数据转发给远程服务器
            base.NewRequestReceived += (session, info) => session.PushRequestToRemoteServer(info.Body, 0, info.Body.Length);

            //默认的策略
            ServerLoadStrategy = new ConfigServerLoadStrategy();
            ForwardStrategy = new PollingForwardStrategy();
        }

        private IServerInfo CreateNewSessionHandler()
        {
            return ForwardStrategy.GetServerInfo(ServerLoadStrategy.GetServers()); 
        }
    }
}
