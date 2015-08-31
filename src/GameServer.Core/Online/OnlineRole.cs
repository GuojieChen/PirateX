using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Online
{
    public class OnlineRole  :IOnlineRole
    {
        public long Id { get; set; }
        public int ServerId { get; set; }
        public string ServerName { get; set; }
        public string SessionID { get; set; }
    }
}
