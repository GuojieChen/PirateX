using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Online;

namespace PirateX.UnitTest
{
    public class OnlineRole:IOnlineRole
    {
        public long Id { get; set; }
        public int Did { get; set; }
        public string HotName { get; set; }
        public string SessionID { get; set; }
    }
}
