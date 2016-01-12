using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Autofac;
using PirateX.Core;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Protocol;
using PirateX.Protocol.V1;
using StackExchange.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX
{

    public abstract class PServer<TSession, TDistrictConfig, TOnlineRole> : AppServer<TSession, IGameRequestInfo>, IGameServer<TDistrictConfig>
        where TSession : PSession<TSession>, new()
        where TDistrictConfig : IDistrictConfig
        where TOnlineRole : class, IOnlineRole, new()
    {
        /// <summary> 后台工作线程列表
        /// </summary>
        protected static readonly IList<Thread> Workers = new List<Thread>();
        public IServerContainer<TDistrictConfig> ServerContainer { get; set; }

        public ILifetimeScope Ioc { get; private set; }

        protected ConnectionMultiplexer MqServer = null;

        protected ISubscriber Subscriber;

        protected PServer(IServerContainer<TDistrictConfig> serverContainer, IReceiveFilterFactory<IGameRequestInfo> receiveFilterFactory) : base(receiveFilterFactory)
        {
            ServerContainer = serverContainer;
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

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"DefaultCulture\t:\t{Thread.CurrentThread.CurrentCulture}");
            }

            #region SERVER IOC

            var builder = new ContainerBuilder();

            MqServer = ConnectionMultiplexer.Connect(ServerContainer.Settings.RedisHost);
            Subscriber = MqServer.GetSubscriber();
            Subscriber.SubscribeAsync(new RedisChannel(Dns.GetHostName().Trim('\''), RedisChannel.PatternMode.Literal),
                (x, y) =>
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"channel:{x},value:{y}");
                });

            //Redis连接池
            builder.Register(c => ConnectionMultiplexer.Connect(ServerContainer.Settings.RedisHost))
                .As<ConnectionMultiplexer>()
                .SingleInstance();

            //在线管理
            builder.Register(c => new RedisOnlineManager<TOnlineRole>(c.Resolve<ConnectionMultiplexer>())
            {
                Serializer = c.Resolve<IRedisSerializer>()
            })
                .As<IOnlineManager<TOnlineRole>>()
                .SingleInstance();

            builder.Register(c => ConfigAssembly()).Named<Assembly>("ConfigAssembly");
            //默认的包解析器
            builder.Register(c => new JsonPackage()).As<IProtocolPackage<IGameRequestInfo>>();
            //全局Redis序列化/反序列化方式
            builder.Register(c => new ProtobufRedisSerializer()).As<IRedisSerializer>().SingleInstance();

            //builder.Register(c => new ServiceStackDatabaseFactory())

            builder.Register(c => rootConfig).As<IRootConfig>().SingleInstance();
            IocConfig(builder);
            Ioc = builder.Build().BeginLifetimeScope();

            #endregion

            ServerContainer.ServerIoc = Ioc;
            ServerContainer.InitContainers();

            RedisDataBaseExtension.RedisSerilazer = Ioc.Resolve<IRedisSerializer>();

            return base.Setup(rootConfig, config);
        }

        public abstract Assembly ConfigAssembly();

        public abstract void IocConfig(ContainerBuilder builder);

        public override bool Start()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("Starting RedisMqServer");

            foreach (var thread in Workers)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"Starting Worker\t:\t{thread.Name}");
                thread.Start();
            }

            return base.Start();
        }

        public override void Stop()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("Stopping...");

            foreach (var thread in Workers)
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

            base.Stop();

            if (Logger.IsDebugEnabled)
                Logger.Debug("Close redis");
            MqServer.Close();
            Ioc.Resolve<ConnectionMultiplexer>().Close();
        }

        protected override void OnSessionClosed(TSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);

            if ((reason == CloseReason.ClientClosing || reason == CloseReason.TimeOut) && session.Rid > 0)
            {
                var onlineManager = this.Ioc.Resolve<IOnlineManager<TOnlineRole>>();
                onlineManager.Logout(session.Rid, session.SessionID);
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"Set role offline\t:\t {session.Rid}\t:{session.SessionID}\t{reason}");
            }
        }
    }
}
