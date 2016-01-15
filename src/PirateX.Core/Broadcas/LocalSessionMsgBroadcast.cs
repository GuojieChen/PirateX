using System;

namespace PirateX.Core.Broadcas
{
    public class LocalSessionMsgBroadcast
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
