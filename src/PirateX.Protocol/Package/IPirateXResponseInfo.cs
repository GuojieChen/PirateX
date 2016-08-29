using System.Collections.Specialized;

namespace PirateX.Protocol.Package
{
    public interface IPirateXResponseInfo
    {
        NameValueCollection Headers { get; set; }
        bool AddHeader(string key, string value);

        byte[] GetHeaderBytes();
    }
}