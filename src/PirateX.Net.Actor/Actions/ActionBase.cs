using System.Data;
using Autofac;
using NetMQ;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Protocol.Package;
using StackExchange.Redis;
using Topshelf.Logging;

namespace PirateX.Net.Actor.Actions
{
    public abstract class ActionBase:IAction
    {
        public IMessageSender MessageSender { get; set; }
        public LogWriter Logger { get; set; }

        public IRedisSerializer RedisSerializer => ServerReslover.Resolve<IRedisSerializer>();
        public IDatabase Redis => Reslover.Resolve<IDatabase>();
        public IDbConnection DbConnection => Reslover.Resolve<IDbConnection>();
        public ILifetimeScope ServerReslover { get; set; }
        public ILifetimeScope Reslover { get; set; }
        public virtual string Name { get; set; }
        public IOnlineRole OnlieRole { get; set; }
        public ActorContext Context { get; set; }
        public abstract void Execute();

        public void Dispose()
        {
            Reslover?.Dispose();
        }
    }
}
