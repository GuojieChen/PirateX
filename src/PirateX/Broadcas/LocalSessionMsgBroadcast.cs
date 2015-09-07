using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Cointainer;

namespace PirateX.Broadcas
{
    public class LocalSessionMsgBroadcast<TGameServerConfig> : IMsgBroadcast where TGameServerConfig : IGameServerConfig
    {
        private readonly IGameServer<IGameServerConfig> _server;


        public void Send<T>(T msg, params long[] rids)
        {
            throw new NotImplementedException();
        }

        public void SendToServer<T>(T msg, params int[] serverId)
        {
            throw new NotImplementedException();
        }
    }
}
