using System;
using System.Net;
using ServiceStack.Common.Utils;
using ServiceStack.Redis;

namespace PirateX.Online
{
    public class RedisOnlineManager<TOnlineRole> : IOnlineManager<TOnlineRole>
        where TOnlineRole : class, IOnlineRole, new()
    {
        private readonly IRedisClientsManager _redisClientsManager;
        private readonly string _urnServerHash;
        /// <summary>
        /// 在线管理构造器
        /// </summary>
        /// <param name="redisClientsManager"></param>
        /// <param name="serverName">server名称</param>
        public RedisOnlineManager(IRedisClientsManager redisClientsManager, string serverName)
        {
            if (redisClientsManager == null)
                throw new ArgumentNullException(nameof(redisClientsManager));

            if (string.IsNullOrEmpty(serverName))
                throw new ArgumentNullException(nameof(serverName));

            _redisClientsManager = redisClientsManager;
            _urnServerHash = $"core.online:{serverName}";
        }

        public RedisOnlineManager(IRedisClientsManager redisClientsManager) : this(redisClientsManager, Dns.GetHostName().Trim('"'))
        {
            _redisClientsManager = redisClientsManager;
        }


        public void ServerOnline()
        {
            using (var redis = _redisClientsManager.GetClient())
                redis.SetEntryInHash(_urnServerHash, "0", "server");
        }

        public void ServerOffline()
        {
            using (var redis = _redisClientsManager.GetClient())
                redis.Remove(_urnServerHash);
        }

        public void Login(TOnlineRole onlineRole)
        {
            if (onlineRole == null)
                return;

            if (onlineRole.Id <= 0)
                return;

            var urn = onlineRole.CreateUrn();

            using (var redis = _redisClientsManager.GetClient())
            {
                using (var trans = redis.CreateTransaction())
                {
                    trans.QueueCommand(r=>r.Store(onlineRole));
                    trans.QueueCommand(r=>r.SetEntryInHash(_urnServerHash, Convert.ToString(onlineRole.Id), urn));
                    trans.Commit();
                }
            }
        }

        public void Logout(long rid, string sessionid)
        {
            if (rid <= 0)
                return; 


            using (var redis = _redisClientsManager.GetClient())
            {
                var or = redis.GetById<TOnlineRole>(rid); 
                if (or != null && Equals(or.SessionID, sessionid))
                {
                    using (var trans = redis.CreateTransaction())
                    {
                        trans.QueueCommand(r=>r.DeleteById<TOnlineRole>(rid));
                        trans.QueueCommand(r=> r.RemoveEntryFromHash(_urnServerHash, Convert.ToString(rid)));
                        trans.Commit();
                    }
                }
            }
        }

        public bool IsOnline(long rid)
        {
            return GetOnlineRole(rid) != null; 
        }

        public TOnlineRole GetOnlineRole(long rid)
        {
            using (var redis = _redisClientsManager.GetClient())
            {
                return redis.GetById<TOnlineRole>(rid); 
            }
        }
    }
}
