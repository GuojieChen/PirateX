using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using PirateX.SLB.ForwardStrategy;
using PirateX.SLB.ServerLoadStrategy;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.SLB
{
    public class SLBServer:AppServer<SLBSession,BinaryRequestInfo>
    {
        /// <summary> 服列表加载策略 
        /// </summary>
        public IServerLoadStrategy  ServerLoadStrategy { get; private set; }

        public IForwardStrategy ForwardStrategy { get; set; }

        private readonly IDictionary<TcpClientSession,IServerInfo> _proxyServers = new Dictionary<TcpClientSession, IServerInfo>();

        private Timer _pingTimer;
        private int _pintInterval;

        public SLBServer() : this(new ConfigServerLoadStrategy())
        {
            
        }

        public SLBServer(IServerLoadStrategy serverLoadStrategy) : base(new SLBFilterFactory())
        {
            //默认的策略
            ServerLoadStrategy = serverLoadStrategy ?? new ConfigServerLoadStrategy();
            ForwardStrategy = new MinConnectionForwardStrategy();

            //创建远程代理
            base.NewSessionConnected += session =>
            {
                if (session.CreateNewSessionHandler == null)
                    session.CreateNewSessionHandler += () => ForwardStrategy.GetServerInfo(ServerLoadStrategy.GetServers());

                session.ConnectProxyServer();
            };
            //将代理收到的数据转发给远程服务器
            base.NewRequestReceived += (session, info) => session.PushRequestToRemoteServer(info.Body, 0, info.Body.Length);
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            foreach (var server in ServerLoadStrategy.GetServers())
                _proxyServers.Add(BuildProxyServerSession(new IPEndPoint(IPAddress.Parse(server.Ip), server.Port)), server);

            foreach (var session in _proxyServers.Keys)
                session.Connect();

            _pintInterval = int.Parse(config.Options.Get("pingInterval") ?? "10"); 

            return base.Setup(rootConfig, config);
        }

        private TcpClientSession BuildProxyServerSession(EndPoint endPoint)
        {
            var proxyServer = new AsyncTcpSession2(endPoint);
            proxyServer.Connected += (sender, args) =>
            {
                var cs = (TcpClientSession)sender;

                if (_proxyServers.ContainsKey(cs))
                {
                    var server = _proxyServers[cs] ;//.Ping= true;
                    server.Ping = true; 

                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Server {server.Ip}:{server.Port}\t connected!");
                }
            };
            proxyServer.Closed += (sender, args) =>
            {
                var cs = (TcpClientSession)sender;

                if (_proxyServers.ContainsKey(cs))
                {
                    var server = _proxyServers[cs]; 
                    server.Ping = false;

                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Ping {server.Ip}:{server.Port}\t\t closed!");
                }
            };

            return proxyServer;
        }


        public override bool Start()
        {
            if(_pingTimer==null)
                _pingTimer = new Timer(PingIntervalCallback,null,0,_pintInterval * 1000);

            return base.Start();
        }

        public override void Stop()
        {
            if (_pingTimer != null)
            {
                _pingTimer.Dispose();
                _pingTimer = null;
            }

            base.Stop();
        }

        private void PingIntervalCallback(object state)
        {
            if(Logger.IsDebugEnabled)
                Logger.Debug($"Start ping....");
            
            var servers = new List<TcpClientSession>(_proxyServers.Keys);

            for (int i = 0; i < servers.Count; i++)
            {
                var server = servers[i];
                if (server.IsConnected)
                {
                    var c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Ping {server}\tOk!");
                    Console.ForegroundColor = c; 
                    continue;
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Warn($"Ping {server}\t ... ");
                }

                var serverInfo = _proxyServers[server];
                _proxyServers.Remove(server);

                var newServerProxy =
                    BuildProxyServerSession(new IPEndPoint(IPAddress.Parse(serverInfo.Ip), serverInfo.Port)); 
                _proxyServers.Add(newServerProxy, serverInfo);

                newServerProxy.Connect();
            }
        }

    }
}
