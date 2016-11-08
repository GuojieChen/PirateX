using System;
using System.Net;
using Autofac;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using StackExchange.Redis;

namespace PirateX.Core.Online
{
    public class RedisOnlineManager<TOnlineRole> : IOnlineManager
        where TOnlineRole : class, IOnlineRole, new()
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        /// <summary> 序列化方式
        /// </summary>
        public ProtobufRedisSerializer Serializer { get; set; }

        private TimeSpan Expiry { get; set; }

        public RedisOnlineManager(ConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer == null)
                throw new ArgumentNullException(nameof(connectionMultiplexer));

            _connectionMultiplexer = connectionMultiplexer;
            Serializer = new ProtobufRedisSerializer();

            Expiry = new TimeSpan(1,0,0,0);//1 day
        }




        public void Login(IOnlineRole onlineRole)
        {
            if (onlineRole == null)
                return;

            if (onlineRole.Id <= 0)
                return;

            var urn = GetUrnOnlineRole(onlineRole.Id);

            var db = _connectionMultiplexer.GetDatabase();

            var trans = db.CreateTransaction();

            trans.StringSetAsync(urn, Serializer.Serilazer(onlineRole), Expiry);
            trans.StringSetAsync(GetUrnOnlineRole(onlineRole.SessionId), urn, Expiry);

            trans.HashSetAsync(GetDidUrn(onlineRole.Did), Convert.ToString(onlineRole.Id), urn);//TODO 需要定时清理
            trans.Execute();
        }

        public void Logout(long rid, string sessionid)
        {
            if (rid <= 0)
                return;

            var urn = $"core:onlinerole:{rid}";

            var db = _connectionMultiplexer.GetDatabase();
            var onlineRoleStr = db.StringGet(urn);
            if (!onlineRoleStr.HasValue)
                return;
            var onlineRole = Serializer.Deserialize<TOnlineRole>(onlineRoleStr);
            if (Equals(onlineRole.SessionId, sessionid))
            {
                var trans = db.CreateTransaction();
                //trans.AddCondition(Condition.StringEqual())
                trans.KeyDeleteAsync(urn);
                trans.HashDeleteAsync(GetDidUrn(onlineRole.Did), Convert.ToString(rid));
                trans.Execute();
            }
        }

        public bool IsOnline(long rid)
        {
            var urn = GetUrnOnlineRole(rid);
            var db = _connectionMultiplexer.GetDatabase();

            return db.KeyExists(urn);
        }

        public IOnlineRole GetOnlineRole(long rid)
        {
            var urn = GetUrnOnlineRole(rid);
            var db = _connectionMultiplexer.GetDatabase();
            var onlineRoleStr = db.StringGet(urn);
            if (!onlineRoleStr.HasValue)
                return default (TOnlineRole);
            return Serializer.Deserialize<TOnlineRole>(onlineRoleStr);
        }

        public IOnlineRole GetOnlineRole(string sessionid)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var urn = db.StringGet(GetUrnOnlineRole(sessionid));
            if(!urn.HasValue)
                return default(TOnlineRole);
            var data = db.StringGet(urn.ToString());


            return Serializer.Deserialize<TOnlineRole>(data);
        }

        private static string GetUrnOnlineRole(long rid)
        {
            return $"core:onlinerole:{rid}";
        }

        private static string GetUrnOnlineRole(string sessionid)
        {
            return $"core:onlinerole_sessionid:{sessionid}";
        }

        private static string GetDidUrn(int did)
        {
            return $"core:onlinerole_dids:{did}";
        }
    }
}
