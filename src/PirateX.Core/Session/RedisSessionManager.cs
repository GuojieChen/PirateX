using System;
using System.Threading.Tasks;
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

            Expiry = new TimeSpan(0, 0, 30, 0);// 默认一分钟
        }

        public async void Login(PirateSession pirateSession)
        {
            if (pirateSession == null)
                return;

            if (pirateSession.Id <= 0)
                return;

            var urn = GetUrnOnlineRole(pirateSession.Id);

            var db = _connectionMultiplexer.GetDatabase();

            db.StringSet(urn, Serializer.Serilazer(pirateSession), Expiry);
            db.StringSet(GetUrnOnlineRole(pirateSession.SessionId), urn, Expiry);
            db.HashSet(GetDidUrn(pirateSession.Did), Convert.ToString(pirateSession.Id), urn);//TODO 需要定时清理
        }

        public async void Logout(long rid)
        {
            if (rid <= 0)
                return;

            var urn = $"core:onlinerole:{rid}";

            var db = _connectionMultiplexer.GetDatabase();

            var onlineRoleStr = db.StringGet(urn);
            if (!onlineRoleStr.HasValue)
                return;
            var onlineRole = Serializer.Deserialize<PirateSession>(onlineRoleStr);

            db.KeyDelete(urn);
            db.HashDelete(GetDidUrn(onlineRole.Did), Convert.ToString(rid));
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
                return default(PirateSession);
            return Serializer.Deserialize<PirateSession>(onlineRoleStr);
        }

        public PirateSession GetOnlineRole(string sessionid)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var urn = db.StringGet(GetUrnOnlineRole(sessionid));
            if (!urn.HasValue)
                return default(PirateSession);
            var data = db.StringGet(urn.ToString());

            if(data == RedisValue.Null)
                return default(PirateSession);


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
