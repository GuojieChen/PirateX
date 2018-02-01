using System.Collections.Specialized;

namespace PirateX.Protocol
{
    public interface IPirateXResponseInfo
    {
        NameValueCollection Headers { get; set; }
        bool AddHeader(string key, string value);

        byte[] GetHeaderBytes();

        byte[] ContentBytes { get; set; }
    }
}