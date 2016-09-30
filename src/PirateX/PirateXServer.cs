﻿using Autofac;
using Newtonsoft.Json;
using PirateX.Command;
using PirateX.Command.System;
using PirateX.Core;
using PirateX.Core.Broadcas;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using PirateX.Sync.ProtoSync;
using StackExchange.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using PirateX.Core.Container;

namespace PirateX
{
    public abstract class PirateXServer<TSession, TOnlineRole> : AppServer<TSession, IPirateXRequestInfo>, IPirateXServer
        where TSession : PirateXSession<TSession>, new()
        where TOnlineRole : class, IOnlineRole, new()
    {
        /// <summary> 后台工作线程列表
        /// </summary>
        protected static readonly IList<Thread> Workers = new List<Thread>();
        public IServerContainer ServerContainer { get; set; }
        public IDictionary<long, string> LoggingSet { get; set; }

        protected ConnectionMultiplexer MqServer = null;

        protected ISubscriber Subscriber;

        protected PirateXServer(IServerContainer serverContainer, IReceiveFilterFactory<IPirateXRequestInfo> receiveFilterFactory) : base(receiveFilterFactory)
        {
            ServerContainer = serverContainer;

            LoggingSet = new Dictionary<long, string>();
        }

        protected override bool SetupCommands(Dictionary<string, ICommand<TSession, IPirateXRequestInfo>> discoveredCommands)
        {
            var commands = new List<ICommand<TSession, IPirateXRequestInfo>>
                {
                    new SysinfoAction<TSession>(),
                    new NewSeed<TSession>(),
                    new Ping<TSession>(),
                    new KeepAlive<TSession>(),
                };

            commands.ForEach(c => discoveredCommands.Add(c.Name, c));

            if (!base.SetupCommands(discoveredCommands))
                return false;
            return true;
        }

        protected override void OnNewSessionConnected(TSession session)
        {
            base.OnNewSessionConnected(session);

            session.ProtocolPackage = ServerContainer.ServerIoc.Resolve<IProtocolPackage>();
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

            //默认数据返回包装器
            builder.Register(c => new JsonResponseConvert()).As<IResponseConvert>().SingleInstance();
            //默认的包解析器
            builder.Register(c => new ProtocolPackage(c.Resolve<IResponseConvert>())).As<IProtocolPackage>();


            builder.Register(c => rootConfig).As<IRootConfig>().SingleInstance();
            //默认消息广播
            builder.Register(c => new DefaultMessageBroadcast()).SingleInstance();

            builder.Register(c => new ProtobufService()).As<IProtoService>().SingleInstance();
            //TODO 默认消息推送（应用级）
            //builder.Register(c =>)
            IocConfig(builder);

            //ServerIoc = builder.Build().BeginLifetimeScope();

            #endregion
            
            ServerContainer.InitContainers(builder);

            RedisDataBaseExtension.RedisSerilazer = ServerContainer.ServerIoc.Resolve<IRedisSerializer>();

            ServerContainer.ServerIoc.Resolve<IProtoService>().Init(ServerContainer.ContainerSetting.EntityAssembly);

            return base.Setup(rootConfig, config);
        }
        
        public abstract Assembly ConfigAssembly();

        public virtual void IocConfig(ContainerBuilder builder)
        {
            
        }

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

            return base.Start();
        }

        #region Subscribe
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
            ServerContainer.ServerIoc.Resolve<ConnectionMultiplexer>().Close();
        }

        protected override void OnSessionClosed(TSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);

            if ((reason == CloseReason.ClientClosing || reason == CloseReason.TimeOut) && session.Rid > 0)
            {
                var onlineManager = this.ServerContainer.ServerIoc.Resolve<IOnlineManager<TOnlineRole>>();
                onlineManager.Logout(session.Rid, session.SessionID);
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"Set role offline\t:\t {session.Rid}\t:{session.SessionID}\t{reason}");
            }
        }

        #region 请求结果的缓存
        private static string GetRequestKey(long rid, string c)
        {
            return $"sys:request:{rid}:{c}";
        }

        private static string GetRequestListKey(long rid)
        {
            return $"sys:requestlist:{rid}";
        }

        public virtual bool ExistsReqeust(IPirateXSession session, string c)
        {
            var rid = session.Rid;

            var db = ServerContainer.ServerIoc.Resolve<IDatabase>();
            if (db == null)
                return false;

            var key = GetRequestKey(rid, c);
            var listkey = GetRequestListKey(rid);

            if (db.ListLength(listkey) <= 0)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"RID : {rid},Request.Length = 0");

                return false;
            }

            var list = db.ListRange(listkey);

            if (list.Any(item => Equals(item.ToString(), key.ToString())))
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"RID : {rid},Request.[{key}] exists!");

                return true;
            }

            if (Logger.IsDebugEnabled)
                Logger.Debug($"RID : {rid},Request.[{key}] not exists!");

            return false;
        }

        public virtual void StartRequest(IPirateXSession session, string c)
        {
            var rid = session.Rid;
            if (rid <= 0 || string.IsNullOrEmpty(c))
                return;

            var db = session.Reslover.Resolve<IDatabase>();
            if (db == null)
                return;

            var onlineManager = ServerContainer.ServerIoc.Resolve<IOnlineManager<TOnlineRole>>();
            var onlineRole = onlineManager.GetOnlineRole(rid);
            if (onlineRole != null && !Equals(onlineRole.SessionID, session.SessionID))
                throw new PirateXException(StatusCode.ReLogin);

            var key = GetRequestKey(rid, c);
            var listkey = GetRequestListKey(rid);

            db.ListLeftPush(listkey, key);

            if (Logger.IsDebugEnabled)
                Logger.Debug($"StartRequest ,ListLeftPush({listkey},{key})");

            if (db.ListLength(listkey) > 4)
            {
                var removekey = db.ListLeftPop(listkey); 
                if (!string.IsNullOrEmpty(removekey))
                    db.KeyDelete(removekey.ToString());

                if (Logger.IsDebugEnabled)
                    Logger.Debug($"StartRequest ,ListLeftPop({listkey}) and KeyDelete({removekey})");
            }
        }

        public virtual void EndRequest(IPirateXSession session, string c, object response)
        {
            var rid = session.Rid;
            if (rid <= 0 || string.IsNullOrEmpty(c))
                return;

            var db = session.Reslover.Resolve<IDatabase>();
            if (db == null)
                return;

            var key = GetRequestKey(rid, c);

            var setOk = db.Set(key, response, new TimeSpan(0, 0, 1, 50));

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"EndRequest, set response {setOk}");
                Logger.Debug($"EndRequest,Response is {JsonConvert.SerializeObject(response)}");
            }
        }

        public virtual TResonse GetResponse<TResonse>(IPirateXSession session, string c)
        {
            var rid = session.Rid;
            var key = GetRequestKey(rid, c);

            var db = session.Reslover.Resolve<IDatabase>();
            if (db == null)
                return default(TResonse);

            return db.Get<TResonse>(key);
        }
        #endregion
    }
}
