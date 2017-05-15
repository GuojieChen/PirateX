﻿using System.Data;
using Autofac;
using NLog;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Core.Session;
using StackExchange.Redis;

namespace PirateX.Core.Actor
{
    public abstract class ActionBase:IAction
    {
        public IMessageSender MessageSender { get; set; }
        public Logger Logger { get; set; }

        public IRedisSerializer RedisSerializer => ServerReslover.Resolve<IRedisSerializer>();
        public virtual IDatabase Redis => Reslover.Resolve<IDatabase>();
        public IDbConnection DbConnection => Reslover.Resolve<IDbConnection>();
        public ILifetimeScope ServerReslover { get; set; }
        public ILifetimeScope Reslover { get; set; }
        public virtual string Name { get; set; }
        //public PirateSession Session { get; set; }
        public ActorContext Context { get; set; }
        public abstract void Execute();

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
            Reslover?.Dispose();
        }
    }
}