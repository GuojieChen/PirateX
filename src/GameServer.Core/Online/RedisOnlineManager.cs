using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Redis;

namespace GameServer.Core.Online
{
    public class RedisOnlineManager : IOnlineManager
    {
        private readonly IRedisClientsManager _redisClientsManager;
        private string _serverName;

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
            _serverName = serverName;
            _urnServerHash = $"core.online:{_serverName}";
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

        public void Login(IOnlineRole onlineRole)
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
                var or = redis.GetById<IOnlineRole>(rid); 
                if (or != null && Equals(or.SessionID, sessionid))
                {
                    using (var trans = redis.CreateTransaction())
                    {
                        trans.QueueCommand(r=>r.DeleteById<IOnlineRole>(rid));
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

        public IOnlineRole GetOnlineRole(long rid)
        {
            using (var redis = _redisClientsManager.GetClient())
            {
                return redis.GetById<IOnlineRole>(rid); 
            }
        }
    }
}
