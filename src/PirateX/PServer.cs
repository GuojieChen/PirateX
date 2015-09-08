using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Autofac;
using PirateX.Cointainer;
using PirateX.Online;
using PirateX.Protocol;
using PirateX.Protocol.V1;
using PirateX.Redis.StackExchange.Redis.Ex;
using StackExchange.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX
{

    public abstract class PServer<TSession,TGameServerConfig,TOnlineRole> : AppServer<TSession, IGameRequestInfo>, IGameServer<TGameServerConfig>
        where TSession : PSession<TSession>, new() 
        where TGameServerConfig : IGameServerConfig
        where TOnlineRole : class, IOnlineRole,new()
    {
        /// <summary> 后台工作线程列表
        /// </summary>
        protected static readonly IList<Thread> WorkerList = new List<Thread>();
        public IGameContainer<TGameServerConfig> GameContainer { get; set; }

        public ILifetimeScope Ioc { get; private set; }

        protected ConnectionMultiplexer MqServer = null;

        protected ISubscriber Subscriber;

        public PServer(IGameContainer<TGameServerConfig> gameContainer,IReceiveFilterFactory<IGameRequestInfo> receiveFilterFactory) :base (receiveFilterFactory)
        {
            GameContainer = gameContainer;
        }

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

        protected override void OnNewSessionConnected(TSession session)
        {
            base.OnNewSessionConnected(session);

            session.ProtocolPackage = Ioc.Resolve<IProtocolPackage<IGameRequestInfo>>();
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            var defaultCulture = System.Configuration.ConfigurationManager.AppSettings["defaultCulture"];

            if (!string.IsNullOrEmpty(defaultCulture))
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(defaultCulture);

            var serversStr = config.Options.Get("servers");
            var redisHost = config.Options.Get("redisHost");

            if (Logger.IsInfoEnabled)
            {
                Logger.Info($"config");
                Logger.Info($"servers\t:\t{serversStr}");
                Logger.Info($"redisHost\t:\t{redisHost}");
                Logger.Info($"DefaultCulture\t:\t{Thread.CurrentThread.CurrentCulture}");
            }

            #region SERVER IOC

            if (Logger.IsDebugEnabled)
                Logger.Debug("IocConfig");
            var builder = new ContainerBuilder();

            MqServer = ConnectionMultiplexer.Connect(redisHost);
            Subscriber = MqServer.GetSubscriber();
            Subscriber.SubscribeAsync(new RedisChannel(Dns.GetHostName().Trim('\''), RedisChannel.PatternMode.Literal),
                (x, y) =>
                {
                    if(Logger.IsDebugEnabled)
                        Logger.Debug($"channel:{x},value:{y}");
                });
            
            //Redis连接池
            builder.Register(c => ConnectionMultiplexer.Connect(redisHost))
                .As<ConnectionMultiplexer>()
                .SingleInstance();
            //在线管理
            builder.Register(c => new RedisOnlineManager<TOnlineRole>(c.Resolve<ConnectionMultiplexer>()))
                .As<IOnlineManager<TOnlineRole>>()
                .SingleInstance();

            builder.Register(c => ConfigAssembly()).Named<Assembly>("ConfigAssembly");
            //默认的包解析器
            builder.Register(c => new JsonPackage()).As<IProtocolPackage<IGameRequestInfo>>();
            //全局Redis序列化/反序列化方式
            builder.Register(c => new JsonRedisSerializer()).As<IRedisSerializer>().SingleInstance();

            builder.Register(c => rootConfig).As<IRootConfig>().SingleInstance();
            builder.Register(c => redisHost).Named<string>("RedisHost");
            IocConfig(builder);
            Ioc = builder.Build().BeginLifetimeScope();

            #endregion

            IEnumerable<int> servers = null;
            if (!string.IsNullOrEmpty(serversStr))
                servers = serversStr.TrimStart('[').TrimEnd(']').Split(new char[] { ',' }).Select(int.Parse);

            if (Logger.IsDebugEnabled)
                Logger.Debug("InitContaners >>>>>>>>>>>>>>>>>>>>>");

            GameContainer.ServiceContainer = Ioc;
            GameContainer.InitContainers(servers?.ToArray());

            RedisDataBaseExtension.RedisSerilazer = Ioc.Resolve<IRedisSerializer>();

            //TODO 启动的时候看是否需要执行脚本 执行完成后进行删除

            //TODO 后台工作队列
            return base.Setup(rootConfig, config);
        }

        public abstract Assembly ConfigAssembly(); 

        public abstract void IocConfig(ContainerBuilder builder);

        public override bool Start()
        {
            if(Logger.IsDebugEnabled)
                Logger.Debug("Starting RedisMqServer");

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

            //_mqServer.Stop();

            base.Stop();
        }

        protected override void OnSessionClosed(TSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);

            if (session.Rid > 0)
            {
                var onlineManager = this.Ioc.Resolve<IOnlineManager<TOnlineRole>>();
                onlineManager.Logout(session.Rid, session.SessionID);
            }

            if (Logger.IsDebugEnabled)
                Logger.Debug($"Set role offline\t:\t {session.Rid}\t:{session.SessionID}\t{reason}");

        }
    }
}
