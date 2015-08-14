using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.PkProtocol
{
    public class PRequestInfo : RequestInfo<JToken>, ISocketRequestInfo
    {

        public PRequestInfo(string key, JToken body, JToken ex,bool r)
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
