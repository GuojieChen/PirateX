using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Online
{
    public class MemoryOnlineManager<TOnlineRole> : IOnlineManager
        where TOnlineRole : class, IOnlineRole, new()
    {
        public void Login(IOnlineRole onlineRole)
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

        public IOnlineRole GetOnlineRole(long rid)
        {
            throw new NotImplementedException();
        }

        public IOnlineRole GetOnlineRole(string sessionid)
        {
            throw new NotImplementedException();
        }
    }
}
