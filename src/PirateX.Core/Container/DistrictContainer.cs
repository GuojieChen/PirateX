using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Builder;
using NLog;
using PirateX.Core.Broadcas;
using PirateX.Core.Config;
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

        private readonly IDictionary<string, IConfigReader> _configReaderDic = new Dictionary<string, IConfigReader>();

        private IContainer _serverContainer;

        public ILifetimeScope ServerIoc { get; set; }

        private IServerSetting _defaultSetting;
        private IServerSetting GetDefaultSeting()
        {
            if (_defaultSetting != null)
                return _defaultSetting;

            var ps = typeof(DefaultServerSetting).GetProperties();
            var defaultServerSetting = Activator.CreateInstance(typeof(DefaultServerSetting), null);

            foreach (var propertyInfo in ps)
            {
                var value = System.Configuration.ConfigurationManager.AppSettings.Get(propertyInfo.Name);
                propertyInfo.SetValue(defaultServerSetting, Convert.ChangeType(value,propertyInfo.PropertyType));
            }

            _defaultSetting = (DefaultServerSetting)defaultServerSetting;

            return _defaultSetting;
        }

        public void InitContainers(ContainerBuilder builder)
        {
            var districtConfigs = GetDistrictConfigs();

            ServerConfig(builder, districtConfigs);

            BuildServerContainer(builder);
            _serverContainer = builder.Build();
            ServerIoc = _serverContainer.BeginLifetimeScope();

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

        private void ServerConfig(ContainerBuilder builder, IEnumerable<IDistrictConfig> configs)
        {

            builder.Register((c, p) => new SqlConnection(p.Named<string>("ConnectionString")))
                .As<IDbConnection>()
                .InstancePerDependency();

            //全局Redis序列化/反序列化方式
            builder.Register(c => new ProtobufRedisSerializer())
                .As<IRedisSerializer>()
                .SingleInstance();

            SetUpConnectionStrings(builder);

            foreach (var config in configs)
            {
                builder.Register(c => config)
                    .Keyed<IDistrictConfig>(config.Id)
                    .AsSelf()
                    .SingleInstance();
            }
        }

        private void SetUpConnectionStrings(ContainerBuilder builder)
        {
            foreach (var kp in GetNamedConnectionStrings())
            {
                builder.Register(c => c.Resolve<IDbConnection>(new NamedParameter("ConnectionString",kp.Value)))
                    .Keyed<IDbConnection>(kp.Key)
                    .InstancePerDependency();
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

        public IEnumerable<IDistrictConfig> GetDistrictConfigs()
        {
            var list = LoadDistrictConfigs();
            //if (Settings.Districts == null)
            //    return list;

            //return list.Where(item => Settings.Districts.Select(d => d.ServerId).Contains(item.Id));

            return list;
        }

        private IContainer LoadDistrictContainer(IDistrictConfig districtConfig)
        {
            if (districtConfig == null)
                return null;

            if (Log.IsTraceEnabled)
                Log.Trace($"Init district container\t{districtConfig.Id}");

            var configtypes = districtConfig.GetType().GetInterfaces();

            var builder = new ContainerBuilder();

            foreach (var type in configtypes)
            {
                var attrs = type.GetCustomAttributes(typeof(DistrictConfigRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                var attr = attrs[0] as DistrictConfigRegisterAttribute;
                if (attr != null)
                    ((IDistrictConfigRegister) Activator.CreateInstance(attr.RegisterType))
                        .Register(builder,districtConfig);
            }

            builder.Register((c, p) => new SqlConnection(p.Named<string>("ConnectionString")))
                .As<IDbConnection>()
                .InstancePerDependency();

            builder.Register(c => districtConfig).As<IDistrictConfig>()
                .SingleInstance();

            builder.Register(c => districtConfig).As<IDistrictConfig>()
                .SingleInstance();

            builder.Register(c => GetConfigAssemblyList())
                .Keyed<List<Assembly>>("ConfigAssemblyList")
                .SingleInstance();

            builder.Register(c => GetServiceAssemblyList())
                .Keyed<List<Assembly>>("ServiceAssemblyList")
                .SingleInstance();

            if (ServerIoc.IsRegistered<IMessageBroadcast>())
                builder.Register(c => ServerIoc.Resolve<IMessageBroadcast>()).As<IMessageBroadcast>().SingleInstance();
            else
                builder.Register(c => new DefaultMessageBroadcast()).As<IMessageBroadcast>().SingleInstance();

            if (ServerIoc.IsRegistered<IPushService>())
                builder.Register(c => ServerIoc.Resolve<IPushService>())
                    .As<IPushService>()
                    .SingleInstance();

            BuildDistrictContainer(builder);

            var services = GetServiceAssemblyList();

            if (!services.Any())
            {
                services.ForEach(x =>
                {
                    builder.RegisterAssemblyTypes(x)
                        .Where(item => typeof(IService).IsAssignableFrom(item))
                        //.WithProperty("Test",123)
                        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                        //.WithProperty(new ResolvedParameter((pi, context) => pi.Name == "Resolver", (pi, ctx) => ctx))
                        .AsSelf()
                        .InstancePerLifetimeScope();
                });
            }

            var container = builder.Build();

            foreach (var type in configtypes)
            {
                var attrs = type.GetCustomAttributes(typeof(DistrictConfigRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                var attr = attrs[0] as DistrictConfigRegisterAttribute;
                if (attr != null)
                    ((IDistrictConfigRegister)Activator.CreateInstance(attr.RegisterType))
                        .SetUp(container,districtConfig);
            }


            return container;
        }

        protected virtual List<Assembly> GetConfigAssemblyList()
        {
            return new List<Assembly>() { typeof(TDistrictContainer).Assembly };
        }

        protected virtual List<Assembly> GetServiceAssemblyList()
        {
            return new List<Assembly>() { typeof(IService).Assembly };
        }

        public virtual List<Assembly> GetEntityAssemblyList()
        {
            return new List<Assembly>() { typeof(IEntity).Assembly };
        }

        public virtual IServerSetting GetServerSetting()
        {
            return GetDefaultSeting();
        }

        #region abstract methods
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
        protected abstract void BuildDistrictContainer(ContainerBuilder builder);

        protected abstract void BuildServerContainer(ContainerBuilder builder);


        public virtual IDictionary<string, string> GetNamedConnectionStrings()
        {
            return new Dictionary<string, string>();
        }

        #endregion


        public void Dispose()
        {
            ServerIoc?.Dispose();
        }
    }
}
