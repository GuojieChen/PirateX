﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Builder;
using Dapper;
using Newtonsoft.Json;
using NLog;
using PirateX.Core.Actor.System;
using PirateX.Core.Broadcas;
using PirateX.Core.Cache;
using PirateX.Core.Config;
using PirateX.Core.DapperMapper;
using PirateX.Core.Domain.Entity;
using PirateX.Core.Push;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Core.Utils;
using StackExchange.Redis;

namespace PirateX.Core.Container
{
    /// <summary> 默认的游戏容器实现
    /// </summary>
    public abstract class DistrictContainer<TDistrictContainer> : IServerContainer
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary> 容器集合
        /// </summary>
        private readonly IDictionary<int, IContainer> _containers = new SortedDictionary<int, IContainer>();

        private readonly object _loadContainerLockHelper = new object();

        private IContainer _serverContainer;

        public ILifetimeScope ServerIoc { get; set; }

        private IServerSetting _defaultSetting;
        protected T GetDefaultSeting<T>() where T : IServerSetting
        {
            var file = $"{AppDomain.CurrentDomain.BaseDirectory}Config\\ServerConfigs.json";

            if (!File.Exists(file))
                return default(T);

            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void InitContainers(ContainerBuilder builder)
        {
            SqlMapper.AddTypeHandler(typeof(long[]), new ArrayJsonMapper<long>());
            SqlMapper.AddTypeHandler(typeof(int[]), new ArrayJsonMapper<int>());
            SqlMapper.AddTypeHandler(typeof(string[]), new ArrayJsonMapper<string>());
            SqlMapper.AddTypeHandler(typeof(double[]), new ArrayJsonMapper<double>());
            SqlMapper.AddTypeHandler(typeof(short[]), new ArrayJsonMapper<short>());
            SqlMapper.AddTypeHandler(typeof(byte[]), new ArrayJsonMapper<byte>());

            SqlMapper.AddTypeHandler(typeof(List<long>), new ListJsonMapper<long>());
            SqlMapper.AddTypeHandler(typeof(List<int>), new ListJsonMapper<int>());
            SqlMapper.AddTypeHandler(typeof(List<string>), new ListJsonMapper<string>());
            SqlMapper.AddTypeHandler(typeof(List<double>), new ListJsonMapper<double>());
            SqlMapper.AddTypeHandler(typeof(List<short>), new ListJsonMapper<short>());
            SqlMapper.AddTypeHandler(typeof(List<byte>), new ListJsonMapper<byte>());

            if (Log.IsTraceEnabled)
                Log.Trace("~~~~~~~~~~ Init server containers ~~~~~~~~~~");

            var districtConfigs = GetDistrictConfigs();

            var serverSetting = GetServerSetting();

            builder.Register(c => serverSetting)
                .AsSelf()
                .As<IServerSetting>()
                .SingleInstance();

            foreach (var type in serverSetting.GetType().GetInterfaces())
            {
                var attrs = type.GetCustomAttributes(typeof(ServerSettingRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                if (attrs[0] is ServerSettingRegisterAttribute attr)
                    ((IServerSettingRegister)Activator.CreateInstance(attr.RegisterType))
                        .Register(builder, serverSetting);
            }

            builder.Register(c => GetConfigAssemblyList())
                .Keyed<List<Assembly>>("ConfigAssemblyList")
                .SingleInstance();

            builder.Register(c => GetServiceAssemblyList())
                .Keyed<List<Assembly>>("ServiceAssemblyList")
                .SingleInstance();

            ServerConfig(builder, districtConfigs);

            BuildServerContainer(builder);
            _serverContainer = builder.Build();
            ServerIoc = _serverContainer.BeginLifetimeScope();


            if (Log.IsTraceEnabled)
                Log.Trace("Setup ServerSetting.");
            foreach (var type in serverSetting.GetType().GetInterfaces())
            {
                var attrs = type.GetCustomAttributes(typeof(ServerSettingRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                if (attrs[0] is ServerSettingRegisterAttribute attr)
                    ((IServerSettingRegister)Activator.CreateInstance(attr.RegisterType))
                        .SetUp(_serverContainer, serverSetting);
            }

            foreach (var kv in GetNamedConnectionStrings())
            {
                if(Log.IsTraceEnabled)
                    Log.Trace($"Initialize Db[{kv.Key}] = {kv.Value}");

                ServerIoc.ResolveKeyed<IDatabaseInitializer>(kv.Key).Initialize(kv.Value);
            }

            if (Log.IsTraceEnabled)
                Log.Trace("Loading distrct containers...");

            foreach (var config in districtConfigs)
            {
                if (_containers.ContainsKey(config.Id))
                    continue;

                var c = LoadDistrictContainer(config);

                if (c == null)
                    continue;

                _containers.Add(config.Id, c);
            }
        }

        protected static string ConnectionStringName = "ConnectionString";

        private void ServerConfig(ContainerBuilder builder, IEnumerable<IDistrictConfig> configs)
        {
            builder.Register((c, p) => new SqlConnection(p.Named<string>(ConnectionStringName)))
                .As<IDbConnection>()
                .InstancePerDependency();

            //全局Redis序列化/反序列化方式
            builder.Register(c => new ProtobufRedisSerializer())
                .As<IRedisSerializer>()
                .SingleInstance();

            if(Log.IsTraceEnabled)
                Log.Trace("SetUp ConnectionStrings...");

            SetUpConnectionStrings(builder);

            foreach (var config in configs)
            {
                builder.Register(c => config)
                    .Keyed<IDistrictConfig>(config.Id)
                    .AsSelf()
                    .SingleInstance();
            }
        }

        /// <summary>
        /// 注册数据库连接
        /// </summary>
        /// <param name="builder"></param>
        private void SetUpConnectionStrings(ContainerBuilder builder)
        {
            foreach (var kp in GetNamedConnectionStrings())
            {
                builder.Register(c => kp.Value)
                    .Keyed<string>(kp.Key)
                    .SingleInstance();

                builder.Register(c => c.Resolve<IDbConnection>(new NamedParameter(ConnectionStringName, kp.Value)))
                    .Keyed<IDbConnection>(kp.Key)
                    .InstancePerDependency();
            }

            //注册对应的 IDatabaseInitializer
            foreach (var kv in GetNamedDatabaseInitializers())
            {
                builder.Register(c => kv.Value)
                    .Keyed<IDatabaseInitializer>(kv.Key)
                    .SingleInstance();
            }
        }

        public IContainer GetDistrictContainer(int districtid)
        {
            if (_containers.ContainsKey(districtid))
                return _containers[districtid];

            lock (_loadContainerLockHelper)
            {
                if (_containers.ContainsKey(districtid))
                    return _containers[districtid];

                var c = LoadDistrictContainer(GetDistrictConfig(districtid));
                if (c == null)
                    return null;

                _containers.Add(districtid, c);

                return c;
            }
        }

        public IContainer ReLoadContainer(int districtid)
        {
            var districtConfig = GetDistrictConfig(districtid);

            var c = LoadDistrictContainer(districtConfig);

            if (_containers.ContainsKey(districtid))
                _containers[districtid] = c;
            else
                _containers.Add(districtid, c);

            return c;
        }

        public abstract IEnumerable<IDistrictConfig> GetDistrictConfigs();

        private IContainer LoadDistrictContainer(IDistrictConfig districtConfig)
        {
            if (districtConfig == null)
                return null;

            if (Log.IsTraceEnabled)
                Log.Trace($"=========== Init district container\t{districtConfig.Id} ===========");

            var configtypes = districtConfig.GetType().GetInterfaces();

            var builder = new ContainerBuilder();

            foreach (var type in configtypes)
            {
                var attrs = type.GetCustomAttributes(typeof(DistrictConfigRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                if (attrs[0] is DistrictConfigRegisterAttribute attr)
                    ((IDistrictConfigRegister)Activator.CreateInstance(attr.RegisterType))
                        .Register(builder, districtConfig);
            }

            builder.Register(c => ServerIoc.Resolve<IRedisSerializer>())
                .As<IRedisSerializer>()
                .SingleInstance();

            builder.Register((c, p) => new SqlConnection(p.Named<string>(ConnectionStringName)))
                .As<IDbConnection>()
                .InstancePerDependency();

            builder.Register(c => new DefaultGameCache(c.Resolve<IRedisSerializer>(), c.Resolve<IDatabase>()))
                .As<IGameCache>()
                .InstancePerDependency();

            SetUpConnectionStrings(builder);

            builder.Register(c => districtConfig)
                .As<IDistrictConfig>()
                .AsImplementedInterfaces()
                .SingleInstance();


            builder.Register(c => GetConfigAssemblyList())
                .Keyed<List<Assembly>>("ConfigAssemblyList")
                .SingleInstance();

            builder.Register(c => GetServiceAssemblyList())
                .Keyed<List<Assembly>>("ServiceAssemblyList")
                .SingleInstance();

            if (ServerIoc.IsRegistered<IMessageBroadcast>())
                builder.Register(c => ServerIoc.Resolve<IMessageBroadcast>())
                    .As<IMessageBroadcast>()
                    .SingleInstance();
            else
                builder.Register(c => new DefaultMessageBroadcast())
                    .As<IMessageBroadcast>()
                    .SingleInstance();

            if (ServerIoc.IsRegistered<IPushService>())
                builder.Register(c => ServerIoc.Resolve<IPushService>())
                    .As<IPushService>()
                    .SingleInstance();

            BuildDistrictContainer(builder);

            var services = GetServiceAssemblyList();

            if (services.Any())
            {
                services.ForEach(x =>
                {
                    foreach (var type in x.GetTypes())
                    {
                        if (type.IsInterface || type.IsAbstract || !typeof(IService).IsAssignableFrom(type))
                            continue;
                        builder.RegisterType(type)
                        //.Where(item => typeof(IService).IsAssignableFrom(item))
                        //.WithProperty("Test",123)
                        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                        //.WithProperty(new ResolvedParameter((pi, context) => pi.Name == "Resolver", (pi, ctx) => ctx))
                        .AsSelf()
                        .SingleInstance();
                    }
                });
            }

            var container = builder.Build();

            foreach (var type in configtypes)
            {
                var attrs = type.GetCustomAttributes(typeof(DistrictConfigRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                if (attrs[0] is DistrictConfigRegisterAttribute attr)
                {
                    if (Log.IsTraceEnabled)
                        Log.Trace($"SetUp DistrictConfig -> {type.Name}");

                    ((IDistrictConfigRegister)Activator.CreateInstance(attr.RegisterType))
                        .SetUp(container, districtConfig);
                }
            }

            if (Log.IsTraceEnabled)
                Log.Trace("");
            return container;
        }

        public virtual List<Assembly> GetConfigAssemblyList()
        {
            return new List<Assembly>() { typeof(TDistrictContainer).Assembly, typeof(TDistrictContainer).Assembly };
        }

        public virtual List<Assembly> GetServiceAssemblyList()
        {
            return new List<Assembly>() { typeof(IService).Assembly, typeof(TDistrictContainer).Assembly };
        }

        public virtual List<Assembly> GetApiAssemblyList()
        {
            return new List<Assembly>() { typeof(TDistrictContainer).Assembly };
        }

        public virtual List<Assembly> GetEntityAssemblyList()
        {
            return new List<Assembly>() { typeof(IEntity).Assembly, typeof(TDistrictContainer).Assembly };
        }

        public virtual IServerSetting GetServerSetting()
        {
            return GetDefaultSeting<DefaultServerSetting>();
        }

        #region abstract methods

        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IDistrictConfig GetDistrictConfig(int id);
        /// <summary>
        /// 创建游戏容器
        /// </summary>
        /// <param name="builder"></param>
        protected abstract void BuildDistrictContainer(ContainerBuilder builder);

        protected abstract void BuildServerContainer(ContainerBuilder builder);

        /// <summary>
        /// 公共数据库连接
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<string, string> GetNamedConnectionStrings()
        {
            return new Dictionary<string, string>();
        }


        public virtual IDictionary<string, IDatabaseInitializer> GetNamedDatabaseInitializers()
        {
            return new Dictionary<string, IDatabaseInitializer>();
        }
        #endregion


        public void Dispose()
        {
            ServerIoc?.Dispose();
        }
    }
}
