using System.Data;
using Autofac;
using NLog;
using StackExchange.Redis;

namespace PirateX.Core
{
    public abstract class ActionBase:IAction
    {
        public IMessageSender MessageSender { get; set; }
        public Logger Logger { get; set; }

        public IRedisSerializer RedisSerializer => ServerReslover.Resolve<IRedisSerializer>();
        public virtual IDatabase Redis => Resolver.Resolve<IDatabase>();
        public IDbConnection DbConnection => Resolver.Resolve<IDbConnection>();

        public ISessionManager SessionManager => ServerReslover.Resolve<ISessionManager>();

        public ILifetimeScope ServerReslover { get; set; }
        public ILifetimeScope Resolver { get; set; }
        public virtual string Name { get; set; }
        //public PirateSession Session { get; set; }
        public ActorContext Context { get; set; }
        public abstract void Execute();
        public byte[] ResponseData { get; set; }

        /// <summary>
        /// 获取公共服连接
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected IDbConnection GetPublicDbConnection(string name)
        {
            var dbconnection = ServerReslover.ResolveNamed<IDbConnection>(name);

            return dbconnection;
        }

        public void Dispose()
        {

        }
    }
}
