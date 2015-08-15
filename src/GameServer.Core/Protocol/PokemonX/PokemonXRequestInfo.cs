using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Protocol.PokemonX
{
    public class PokemonXRequestInfo : RequestInfo<JToken>, IPokemonXRequestInfo
    {

        public PokemonXRequestInfo(string key, JToken body, JToken ex,bool r)
            : base(key, body)
        {
            Ex = ex; 
            Body = body;
            R = r;
        }

        public JToken Ex { get; private set; }
        public bool R { get; private set; }
        public new JToken Body { get; private set; }
    }
}
