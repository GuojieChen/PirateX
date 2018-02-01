using System;

namespace PirateX.Core
{
    public class RedisMqMessageBroadcast:IMessageBroadcast
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
