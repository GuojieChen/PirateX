﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Core;
using NLog;
using PirateX.Core.Broadcas;
using PirateX.Core.Config;
using PirateX.Core.Domain.Entity;
using PirateX.Core.Push;
using PirateX.Core.Utils;
using StackExchange.Redis;

namespace PirateX.Core
{
    /// <summary> 默认的游戏容器实现
    /// </summary>
    public abstract class DistrictContainer : IServerContainer
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary> 容器集合
        /// </summary>
        private readonly IDictionary<int, IContainer> _containers = new SortedDictionary<int, IContainer>();

        private readonly object _loadContainerLockHelper = new object();

        private readonly IDictionary<string, IConfigReader> _configReaderDic = new Dictionary<string, IConfigReader>();

        public ILifetimeScope ServerIoc { get; set; }

        public IServerSetting Settings { get; }

        public IContainerSetting ContainerSetting { get; }

        public DistrictContainer(IContainerSetting csetting, IServerSetting settings)
        {
            Settings = settings;
            ContainerSetting = csetting;

            if (ContainerSetting == null)
                throw new ArgumentException("ContainerSetting");

            if (Settings == null)
            {
                if (Log.IsTraceEnabled)
                    Log.Trace("Settings is NULL");

                Settings = GetDefaultSeting();
            }


            if (Log.IsTraceEnabled)
            {
                Log.Trace("Settings values:");
                Log.Trace(Settings.ToLogString());
            }
        }

        public DistrictContainer(IContainerSetting csetting) : this(csetting, null)
        {

        }

        private static DefaultServerSetting GetDefaultSeting()
        {
            var ps = typeof(DefaultServerSetting).GetProperties();
            var defaultServerSetting = Activator.CreateInstance(typeof(DefaultServerSetting), null);

            foreach (var propertyInfo in ps)
            {
                var value = System.Configuration.ConfigurationManager.AppSettings.Get(propertyInfo.Name.ToLower());
                propertyInfo.SetValue(defaultServerSetting, value);
            }

            return (DefaultServerSetting)defaultServerSetting;
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

        public IEnumerable<IDistrictConfig> GetDistrictConfigs()
        {
            var list = LoadDistrictConfigs();
            if (Settings.Districts == null)
                return list;

            return list.Where(item => Settings.Districts.Select(d => d.ServerId).Contains(item.Id));
        }

        private IContainer LoadDistrictContainer(IDistrictConfig districtConfig)
        {
            if (districtConfig == null)
                return null;

            if (Log.IsTraceEnabled)
                Log.Trace($"Init district container\t{districtConfig.Id}");

            var builder = new ContainerBuilder();

            builder.Register(c => districtConfig).As<IDistrictConfig>()
                .SingleInstance();

            builder.Register(c => ConnectionMultiplexer.Connect(districtConfig.Redis))
                .AsSelf();

            builder.Register(c => c.Resolve<ConnectionMultiplexer>().GetDatabase(districtConfig.RedisDb))
                .As<IDatabase>()
                .InstancePerDependency();

            builder.Register(c => districtConfig).As<IDistrictConfig>()
                .SingleInstance();

            if (ServerIoc.IsRegistered<IMessageBroadcast>())
                builder.Register(c => ServerIoc.Resolve<IMessageBroadcast>()).As<IMessageBroadcast>().SingleInstance();
            else
                builder.Register(c => new DefaultMessageBroadcast()).As<IMessageBroadcast>().SingleInstance();

            if (ServerIoc.IsRegistered<IPushService>())
                builder.Register(c => ServerIoc.Resolve<IPushService>())
                    .As<IPushService>()
                    .SingleInstance();

            BuildContainer(builder);

            //默认Config缓存数据处理器
            builder.Register(c =>
            {
                var configDbKey = GetConfigDbKey(districtConfig.ConfigConnectionString);
                if (_configReaderDic.ContainsKey(configDbKey))
                    return _configReaderDic[configDbKey];

                var newReader = new MemoryConfigReader(ContainerSetting.ConfigAssembly);
                _configReaderDic.Add(configDbKey, newReader);
                return newReader;
            })
                .As<IConfigReader>()
                .SingleInstance();

            builder.Register(c => new SqlConnection(districtConfig.ConnectionString))
                .As<IDbConnection>().InstancePerDependency();

            if (ContainerSetting.ServiceAssembly != null)
            {
                builder.RegisterAssemblyTypes(ContainerSetting.ServiceAssembly)
                    .Where(item =>typeof(IService).IsAssignableFrom(item))
                    //.WithProperty("Test",123)
                    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                    //.WithProperty(new ResolvedParameter((pi, context) => pi.Name == "Resolver", (pi, ctx) => ctx))
                    .AsSelf()
                    //.AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            builder.Register(c => GetDistrictDatabaseFactory(districtConfig)).As<IDatabaseFactory>().SingleInstance();

            var container = builder.Build();

            if (Log.IsTraceEnabled)
                Log.Trace("CreateAndAlterTable");
            if (ContainerSetting.EntityAssembly != null)
                container.Resolve<IDatabaseFactory>().CreateAndAlterTable(ContainerSetting.EntityAssembly.GetTypes().Where(item => typeof(IEntity).IsAssignableFrom(item)));

            if (ContainerSetting.ConfigAssembly != null)
                container.Resolve<IConfigReader>().Load(GetConfigDatabaseFactory(districtConfig));

            return container;
        }

        /// <summary>
        /// 获取配置连接的信息摘要
        /// 这个后期是否需要非内置？
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static string GetConfigDbKey(string connectionString)
        {
            var items = connectionString.Split(new char[] { ';' });
            var builder = new StringBuilder();
            foreach (var item in items)
            {
                var ss = item.Split(new char[] { '=' });
                switch (ss[0].Trim().ToLower())
                {
                    case "database":
                    case "server":
                        builder.Append(ss[1].Trim().ToLower());
                        break;
                }
            }

            return builder.ToString();
        }

        public void InitContainers()
        {
            foreach (var config in GetDistrictConfigs())
            {
                if (_containers.ContainsKey(config.Id))
                    continue;

                var c = LoadDistrictContainer(config);

                if (c == null)
                    continue;

                _containers.Add(config.Id, c);
            }
            //运行期间不使用表刷新
            Settings.AlterTable = false;
        }
        /// <summary> 加载配置列表
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IDistrictConfig> LoadDistrictConfigs();
        /// <summary> 获取单个 配置信息 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IDistrictConfig GetDistrictConfig(int id);
        /// <summary>
        /// 创建游戏容器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        public abstract void BuildContainer(ContainerBuilder builder);

        public abstract IDatabaseFactory GetConfigDatabaseFactory(IDistrictConfig config);

        public abstract IDatabaseFactory GetDistrictDatabaseFactory(IDistrictConfig config);
    }


    public interface IContainerSetting
    {
        Assembly ConfigAssembly { get; }

        Assembly EntityAssembly { get; }

        Assembly ServiceAssembly { get; }
    }
}