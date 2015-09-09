using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Cointainer;

namespace PirateX.Broadcas
{
    public class LocalSessionMsgBroadcast<TDistrictConfig> : IMsgBroadcast where TDistrictConfig : Cointainer.IDistrictConfig
    {
        private readonly IGameServer<TDistrictConfig> _server;


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
