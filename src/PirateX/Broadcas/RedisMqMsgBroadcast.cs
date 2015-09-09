using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Broadcas
{
    public class RedisMqMsgBroadcast:IMsgBroadcast
    {
        public void Send<T>(T msg, params long[] rids)
        {
            throw new NotImplementedException();
        }

        public void SendToDistrict<T>(T msg, params int[] districtId)
        {
            throw new NotImplementedException();
        }
    }
}
