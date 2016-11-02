using System.Collections.Generic;
using System.Collections.Specialized;

namespace PirateX.Protocol.Package
{
    public interface IPirateXRequestInfoBase
    {
        string C { get; set; }
        int O { get; set; }
        bool R { get; set; }
        long Timestamp { get; set; }
        string Token { get; set; }
        IDictionary<string,string> Headers { get; set; }
        IDictionary<string, string> QueryString { get; set; }

        IPirateXRequestPackage ToRequestPackage();
    }
}