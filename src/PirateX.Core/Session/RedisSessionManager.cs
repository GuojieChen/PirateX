using System;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using StackExchange.Redis;

namespace PirateX.Core.Session
{
    public class RedisOnlineManager : ISessionManager
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        /// <summary> 序列化方式
        /// </summary>
        public ProtobufRedisSerializer Serializer { get; set; }

        private TimeSpan Expiry { get; set; }

        public RedisOnlineManager(ConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            Serializer = new ProtobufRedisSerializer();

            Expiry = new TimeSpan(1,0,0,0);//1 day
        }

        public void Login(PirateSession pirateSession)
        {
            if (pirateSession == null)
                return;

            if (pirateSession.Id <= 0)
                return;

            var urn = GetUrnOnlineRole(pirateSession.Id);

            var db = _connectionMultiplexer.GetDatabase();

            var trans = db.CreateTransaction();

            trans.StringSetAsync(urn, Serializer.Serilazer(pirateSession), Expiry);
            trans.StringSetAsync(GetUrnOnlineRole(pirateSession.SessionId), urn, Expiry);

            trans.HashSetAsync(GetDidUrn(pirateSession.Did), Convert.ToString(pirateSession.Id), urn);//TODO 需要定时清理
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
            var onlineRole = Serializer.Deserialize<PirateSession>(onlineRoleStr);
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

        public PirateSession GetOnlineRole(long rid)
        {
            var urn = GetUrnOnlineRole(rid);
            var db = _connectionMultiplexer.GetDatabase();
            var onlineRoleStr = db.StringGet(urn);
            if (!onlineRoleStr.HasValue)
                return default (PirateSession);
            return Serializer.Deserialize<PirateSession>(onlineRoleStr);
        }

        public PirateSession GetOnlineRole(string sessionid)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var urn = db.StringGet(GetUrnOnlineRole(sessionid));
            if(!urn.HasValue)
                return default(PirateSession);
            var data = db.StringGet(urn.ToString());


            return Serializer.Deserialize<PirateSession>(data);
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
