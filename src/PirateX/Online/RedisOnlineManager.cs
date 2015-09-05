using System;
using System.Net;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace PirateX.Online
{
    public class RedisOnlineManager<TOnlineRole> : IOnlineManager<TOnlineRole>
        where TOnlineRole : class, IOnlineRole, new()
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly string _urnServerHash;
        /// <summary>
        /// 在线管理构造器
        /// </summary>
        /// <param name="connectionMultiplexer"></param>
        /// <param name="serverName">server名称</param>
        public RedisOnlineManager(ConnectionMultiplexer connectionMultiplexer, string serverName)
        {
            if (connectionMultiplexer == null)
                throw new ArgumentNullException(nameof(connectionMultiplexer));

            if (string.IsNullOrEmpty(serverName))
                throw new ArgumentNullException(nameof(serverName));

            _connectionMultiplexer = connectionMultiplexer;
            _urnServerHash = $"core.online:{serverName}";
        }

        public RedisOnlineManager(ConnectionMultiplexer connectionMultiplexer) : this(connectionMultiplexer, Dns.GetHostName().Trim('"'))
        {
            _connectionMultiplexer = connectionMultiplexer;
        }


        public void ServerOnline()
        {
            var db = _connectionMultiplexer.GetDatabase();
            db.HashSet(_urnServerHash, "0", "server");
        }

        public void ServerOffline()
        {
            var db = _connectionMultiplexer.GetDatabase();
            db.HashDelete(_urnServerHash, "0");
        }

        public void Login(TOnlineRole onlineRole)
        {
            if (onlineRole == null)
                return;

            if (onlineRole.Id <= 0)
                return;

            var urn = $"core:onlinerole:{onlineRole.Id}";

            var db = _connectionMultiplexer.GetDatabase();
            var trans = db.CreateTransaction();

            trans.StringSetAsync(urn, JsonConvert.SerializeObject(onlineRole));
            trans.HashSetAsync(_urnServerHash, Convert.ToString(onlineRole.Id), urn);
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
            var onlineRole = JsonConvert.DeserializeObject<TOnlineRole>(onlineRoleStr);
            if (Equals(onlineRole.SessionID, sessionid))
            {
                var trans = db.CreateTransaction();
                //trans.AddCondition(Condition.StringEqual())
                trans.KeyDeleteAsync(urn);
                trans.HashDeleteAsync(_urnServerHash, Convert.ToString(rid));
                trans.Execute();
            }
        }

        public bool IsOnline(long rid)
        {
            return GetOnlineRole(rid) != null;
        }

        public TOnlineRole GetOnlineRole(long rid)
        {
            var urn = $"core:onlinerole:{rid}";
            var db = _connectionMultiplexer.GetDatabase();
            var onlineRoleStr = db.StringGet(urn);
            if (!onlineRoleStr.HasValue)
                return default (TOnlineRole);
            return JsonConvert.DeserializeObject<TOnlineRole>(onlineRoleStr);
        }
    }
}
