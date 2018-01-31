using System.Data;
using Autofac;
using StackExchange.Redis;

namespace PirateX.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPublicRepository
    {
        /// <summary>
        /// 
        /// </summary>
        ILifetimeScope Resolver { get; set; }
        /// <summary>
        /// 数据库连接字符串参数
        /// </summary>
        NamedParameter ConnectionStringName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PublicRepository : IPublicRepository
    {
        /// <summary>
        /// 
        /// </summary>
        public ILifetimeScope Resolver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public NamedParameter ConnectionStringName { get; set; }

        protected IDbConnection DbConnection()
        {
            return Resolver.Resolve<IDbConnection>(ConnectionStringName);
        }

        protected IDatabase Redis => Resolver.Resolve<IDatabase>();

        protected IRedisSerializer RedisSerializer => Resolver.Resolve<IRedisSerializer>();
    }
}
