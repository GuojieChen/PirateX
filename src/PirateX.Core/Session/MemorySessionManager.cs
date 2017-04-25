using System;

namespace PirateX.Core.Session
{
    public class MemorySessionManager : ISessionManager
    {
        public void Login(PirateSession pirateSession)
        {
            throw new NotImplementedException();
        }

        public void Logout(long rid, string sessionid)
        {
            throw new NotImplementedException();
        }

        public bool IsOnline(long rid)
        {
            throw new NotImplementedException();
        }

        public PirateSession GetOnlineRole(long rid)
        {
            throw new NotImplementedException();
        }

        public PirateSession GetOnlineRole(string sessionid)
        {
            throw new NotImplementedException();
        }
    }
}
