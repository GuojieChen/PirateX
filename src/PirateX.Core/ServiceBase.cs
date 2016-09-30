using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NLog;
using PirateX.Core.Broadcas;
using PirateX.Core.Config;
using PirateX.Core.Domain.Uow;
using PirateX.Core.Push;
using StackExchange.Redis;
using PirateX.Core.Redis.StackExchange.Redis.Ex;

namespace PirateX.Core
{
    public class ServiceBase : IService
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public ILifetimeScope Resolver { get; set; }

        /// <summary> Redis库操作对象
        /// </summary>
        protected IDatabase RedisDatabase => Resolver.Resolve<IDatabase>();

        /// <summary> 配置读取
        /// </summary>
        protected IConfigReader ConfigReader => Resolver.Resolve<IConfigReader>();

        /// <summary> 消息广播
        /// </summary>
        protected IMessageBroadcast MessageBroadcast => Resolver.Resolve<IMessageBroadcast>();

        /// <summary> 消息推送
        /// </summary>
        protected IPushService PushService => Resolver.Resolve<IPushService>();
        

        protected IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(Resolver);
        }

        protected IUnitOfWork CreateUnitOfWork(string name)
        {
            return new UnitOfWork(Resolver,name);
        }

        //TODO UnitOfWork  Repository
    }
}
