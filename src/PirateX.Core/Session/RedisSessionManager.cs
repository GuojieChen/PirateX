﻿using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace PirateX.Core
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

        public void Login(PirateSession pirateSession)
        {
            if (pirateSession == null)
                return;

            if (pirateSession.Id <= 0)
                return;

            var urn = GetUrnOnlineRole(pirateSession.Id);

            var db = _connectionMultiplexer.GetDatabase();

            db.StringSet(urn, Serializer.Serilazer(pirateSession), Expiry);
            db.StringSet(GetUrnOnlineRole(pirateSession.SessionId), urn, Expiry);
            db.HashSet(GetDidUrn(pirateSession.Did), Convert.ToString(pirateSession.Id), urn);
        }

        public void Logout(long rid)
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
            db.HashDelete(GetFrontendIDDIDUrn(onlineRole.Did), onlineRole.FrontendID);
        }

        public bool IsOnline(long rid)
        {
            var urn = GetUrnOnlineRole(rid);
            var db = _connectionMultiplexer.GetDatabase();

            return db.KeyExists(urn);
        }

        public void Save(PirateSession session)
        {
            var urn = GetUrnOnlineRole(session.Id);
            var db = _connectionMultiplexer.GetDatabase();

            db.StringSet(urn, Serializer.Serilazer(session), Expiry);
        }

        public PirateSession GetSession(long rid)
        {
            var urn = GetUrnOnlineRole(rid);
            var db = _connectionMultiplexer.GetDatabase();
            var onlineRoleStr = db.StringGet(urn);
            if (!onlineRoleStr.HasValue)
                return default(PirateSession);
            return Serializer.Deserialize<PirateSession>(onlineRoleStr);
        }

        public PirateSession GetSession(string sessionid)
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

        public IEnumerable<string> GetFrontendIDListByDid(int did)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return db.HashGetAll(GetFrontendIDDIDUrn(did)).Select(item => Convert.ToString(item));
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

        private static string GetFrontendIDDIDUrn(int did)
        {
            return $"core:frontendid_dids:{did}";
        }
    }
}
