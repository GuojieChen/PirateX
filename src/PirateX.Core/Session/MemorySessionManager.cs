using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PirateX.Core
{
    public class MemorySessionManager : ISessionManager
    {

        private ConcurrentDictionary<long, PirateSession> _sessionDic = new ConcurrentDictionary<long, PirateSession>();
        private ConcurrentDictionary<string, long> _sessionidDic = new ConcurrentDictionary<string, long>();
        public void Login(PirateSession pirateSession)
        {
            _sessionDic.AddOrUpdate(pirateSession.Id, pirateSession, (l, session) => pirateSession);
            _sessionidDic.AddOrUpdate(pirateSession.SessionId, pirateSession.Id, (s, l) => pirateSession.Id);
        }

        public void Logout(long rid)
        {
            PirateSession session;

            _sessionDic.TryRemove(rid, out session);
        }

        public bool IsOnline(long rid)
        {
            return _sessionDic.ContainsKey(rid);
        }

        public void Save(PirateSession session)
        {
            if (_sessionDic.ContainsKey(session.Id))
                _sessionDic[session.Id] = session;
        }

        public PirateSession GetSession(long rid)
        {
            PirateSession session;
            _sessionDic.TryGetValue(rid, out session);

            return session;
        }

        public PirateSession GetSession(string sessionid)
        {
            long rid = 0;
            _sessionidDic.TryGetValue(sessionid, out rid);
            if (rid > 0)
            {
                PirateSession session;
                _sessionDic.TryGetValue(rid, out session);

                return session;
            }

            return null;
        }

        public IEnumerable<string> GetFrontendIDListByDid(int did)
        {
            throw new NotImplementedException();
        }
    }
}
