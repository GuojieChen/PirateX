using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using GameServer.Container;
using GameServer.Core.Protocol;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using IServerConfig = SuperSocket.SocketBase.Config.IServerConfig;

namespace GameServer.Core
{
    public abstract class PServer<TSession, TErrorCode,TGameServerConfig> : AppServer<TSession, IGameRequestInfo>, IGameServer<TGameServerConfig>
        where 
        TSession : PSession<TSession, TErrorCode>, new() where TGameServerConfig : IGameServerConfig
    {
        /// <summary> 后台工作线程列表
        /// </summary>
        protected static readonly IList<Thread> WorkerList = new List<Thread>();
        public IGameContainer<TGameServerConfig> GameContainer { get; set; }

        protected IContainer Container { get; private set; }

        public PServer(IGameContainer<TGameServerConfig> gameContainer,IReceiveFilterFactory<IGameRequestInfo> receiveFilterFactory) :base (receiveFilterFactory)
        {
            GameContainer = gameContainer;
        }

        private RedisMqServer _mqServer => Container.Resolve<RedisMqServer>(); 

        public void Broadcast<TMessage>(TMessage message, IQueryable<long> rids)
        {
            if (message == null || rids == null)
                return;

            this.AsyncRun(() =>
            {
                var sessions = GetAllSessions().Where(item => rids.Contains(item.Rid));
                foreach (var session in sessions)
                    session.SendMessage(message);
            }, exception => Logger.Error(exception));
        }
        

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            var serversStr = config.Options.Get("servers");
            var redisHost = config.Options.Get("redisHost");

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"game Config >>>>>>>>>>>>>>>>>>>>>");
                Logger.Debug($"servers\t:\t{serversStr}");
                Logger.Debug($"redisHost\t:\t{redisHost}");
            }

            #region SERVER IOC

            if (Logger.IsDebugEnabled)
                Logger.Debug("SetServerConfig >>>>>>>>>>>>>>>>>>>>>");
            var builder = new ContainerBuilder();
            builder.Register(c => new RedisMqServer(new PooledRedisClientManager(redisHost.Split(','))))
                .As<RedisMqServer>()
                .SingleInstance();

            builder.Register(c => new PooledRedisClientManager(redisHost.Split(',')))
                .As<IRedisClientsManager>()
                .SingleInstance();

            builder.Register(c => rootConfig).As<IRootConfig>().SingleInstance();
            builder.Register(c => redisHost).Named<string>("RedisHost");
            SetServerConfig(builder);
            Container = builder.Build();

            #endregion

            IEnumerable<int> servers = null;
            if (!string.IsNullOrEmpty(serversStr))
                servers = serversStr.TrimStart('[').TrimEnd(']').Split(new char[] { ',' }).Select(int.Parse);

            if (Logger.IsDebugEnabled)
                Logger.Debug("InitContaners >>>>>>>>>>>>>>>>>>>>>");
            GameContainer.InitContainers(servers?.ToArray());
            
            //TODO 启动的时候看是否需要执行脚本 执行完成后进行删除

            //TODO 后台工作队列



            return base.Setup(rootConfig, config);
        }

        public abstract void SetServerConfig(ContainerBuilder builder);

        public override bool Start()
        {
            if(Logger.IsDebugEnabled)
                Logger.Debug("Starting RedisMqServer");

            _mqServer.Start();

            foreach (var thread in WorkerList)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"Starting Worker\t:\t{thread.Name}");
                thread.Start();
            }

            return base.Start();
        }

        public override void Stop()
        {
            foreach (var thread in WorkerList)
            {
                try
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Stoping Worker\t:\t{thread.Name}");
                    thread.Abort();
                }
                catch (Exception ex)
                {
                    if (Logger.IsErrorEnabled)
                    {
                        Logger.Error($"{thread.Name}");
                        Logger.Error(ex);
                    }
                }
            }

            if (Logger.IsDebugEnabled)
                Logger.Debug("Stoping RedisMqServer");

            _mqServer.Stop();

            base.Stop();
        }
    }
}
