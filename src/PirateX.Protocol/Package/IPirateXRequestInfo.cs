using System.Collections.Specialized;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol.Package
{
    /// <summary> 协议抽象
    /// </summary>
    public interface IPirateXRequestInfo : IRequestInfo
    {
        int O { get; set; }

        bool R { get; set; }

        long Timestamp { get; set; }

        string Token { get; set; }

        NameValueCollection Headers { get; set; }

        NameValueCollection QueryString { get; set; }
    }
}
