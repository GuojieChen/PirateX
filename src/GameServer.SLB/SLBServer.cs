using System;
using GameServer.SLB.ForwardStrategy;
using GameServer.SLB.ServerLoadStrategy;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.SLB
{
    public class SLBServer:AppServer<SLBSession,BinaryRequestInfo>
    {
        /// <summary> 服列表加载策略 
        /// </summary>
        public IServerLoadStrategy  ServerLoadStrategy { get; private set; }

        public IForwardStrategy ForwardStrategy { get; set; }


        public SLBServer() : this(new ConfigServerLoadStrategy())
        {
            
        }

        public SLBServer( IServerLoadStrategy serverLoadStrategy) : base(new SLBFilterFactory())
        {
            //创建远程代理
            base.NewSessionConnected += session =>
            {
                if (session.CreateNewSessionHandler == null)
                    session.CreateNewSessionHandler += () => ForwardStrategy.GetServerInfo(ServerLoadStrategy.GetServers());

                session.ConnectProxyServer();
            };
            //将代理收到的数据转发给远程服务器
            base.NewRequestReceived += (session, info) => session.PushRequestToRemoteServer(info.Body, 0, info.Body.Length);

            //默认的策略
            ServerLoadStrategy = serverLoadStrategy ?? new ConfigServerLoadStrategy();
            ForwardStrategy = new MinConnectionForwardStrategy();
        }
    }
}
