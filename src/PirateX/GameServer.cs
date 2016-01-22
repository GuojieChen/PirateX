using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Autofac;
using PirateX.Core;
using PirateX.Core.Broadcas;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Core.Service;
using PirateX.Filters;
using PirateX.Protocol;
using PirateX.Protocol.V1;
using StackExchange.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX
{

    public abstract class GameServer<TSession, TOnlineRole> : AppServer<TSession, IGameRequestInfo> ,IGameServer
        where TSession : GameSession<TSession>, new()
        where TOnlineRole : class, IOnlineRole, new()
    {
        /// <summary> 后台工作线程列表
        /// </summary>
        protected static readonly IList<Thread> Workers = new List<Thread>();
        public IServerContainer ServerContainer { get; set; }

        public ILifetimeScope Ioc { get; private set; }

        protected ConnectionMultiplexer MqServer = null;

        protected ISubscriber Subscriber;

        protected GameServer(IServerContainer  serverContainer, IReceiveFilterFactory<IGameRequestInfo> receiveFilterFactory) : base(receiveFilterFactory)
        {
            ServerContainer = serverContainer;
        }

        protected override void OnNewSessionConnected(TSession session)
        {
            base.OnNewSessionConnected(session);

            session.ProtocolPackage = Ioc.Resolve<IProtocolPackage>();
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
            //Redis连接池
            builder.Register(c => ConnectionMultiplexer.Connect(ServerContainer.Settings.RedisHost))
                .As<ConnectionMultiplexer>()
                .SingleInstance();

            //在线管理
            builder.Register(c => new RedisOnlineManager<TOnlineRole>(c.Resolve<ConnectionMultiplexer>())
            {
                Serializer = c.Resolve<IRedisSerializer>()
            }).As<IOnlineManager<TOnlineRole>>()
              .SingleInstance();

            //默认的包解析器
            builder.Register(c => new JsonPackage()).As<IProtocolPackage>();
            //全局Redis序列化/反序列化方式
            builder.Register(c => new ProtobufRedisSerializer()).As<IRedisSerializer>().SingleInstance();
            
            builder.Register(c => rootConfig).As<IRootConfig>().SingleInstance();
            //默认消息广播
            builder.Register(c => new DefaultMessageBroadcast()).SingleInstance();
            //TODO 默认消息推送（应用级）
            //builder.Register(c =>)
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
            foreach (var thread in Workers)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"Starting Worker\t:\t{thread.Name}");
                thread.Start();
            }

            MqServer.GetSubscriber()
                .SubscribeAsync(new RedisChannel(Dns.GetHostName(), RedisChannel.PatternMode.Literal), BroadcastToRoleSubscribe);

            Ioc.Resolve<ConnectionMultiplexer>().GetSubscriber()
                .SubscribeAsync(new RedisChannel(KeyStore.SubscribeChannelLogout, RedisChannel.PatternMode.Literal), LogoutSubscribe);
            
            return base.Start();
        }
        #region Subscribe
        private void LogoutSubscribe(RedisChannel channel, RedisValue sessionid)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"logout \t channel:{channel},value:{sessionid}");

            var session = GetSessionByID(sessionid);

            if (session == null)
                return;

            if (!session.Items.ContainsKey(KeyStore.FilterIsLogout))
                session.Items.Add(KeyStore.FilterIsLogout, true);
        }

        private void BroadcastToRoleSubscribe(RedisChannel channel, RedisValue value)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"channel:{channel},value:{value}");
        }
        #endregion

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
