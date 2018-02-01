using System.Collections.Specialized;

namespace PirateX.Protocol
{
    public interface IPirateXRequestInfoBase
    {
        string C { get; set; }
        int O { get; set; }
        bool R { get; set; }
        long Timestamp { get; set; }
        string Token { get; set; }
        NameValueCollection Headers { get; set; }
        NameValueCollection QueryString { get; set; }

        IPirateXPackage ToRequestPackage();
    }
}