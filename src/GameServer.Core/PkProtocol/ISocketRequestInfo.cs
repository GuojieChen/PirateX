using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.PkProtocol
{
    public interface ISocketRequestInfo: IRequestInfo
    {
        JToken Body { get;  }

        bool R { get; }
    }
}
