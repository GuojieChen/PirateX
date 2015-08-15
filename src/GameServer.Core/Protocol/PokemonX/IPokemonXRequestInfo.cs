using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Protocol.PokemonX
{
    public interface IPokemonXRequestInfo: IRequestInfo
    {
        JToken Body { get;  }

        bool R { get; }
    }
}
