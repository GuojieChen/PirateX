using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core;

namespace PirateX.Broadcas
{
    public class LocalSessionMsgBroadcast<TDistrictConfig> : IMsgBroadcast where TDistrictConfig : IDistrictConfig
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
