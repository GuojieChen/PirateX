using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace PirateX.Protocol
{
    public class PirateXResponseInfo : IPirateXResponseInfo
    {
        public NameValueCollection Headers { get; set; }
        public bool AddHeader(string key, string value)
        {
            if (!string.IsNullOrEmpty(Headers[key]))
            {
                Headers.Add(key,value);
                return true;
            }

            return false;
        }

        public byte[] GetHeaderBytes()
        {
            return Encoding.UTF8.GetBytes($"{String.Join("&", Headers.AllKeys.Select(a => a + "=" + Headers[a]))}");
        }

        public byte[] ContentBytes { get; set; }

        public PirateXResponseInfo()
        {
            Headers = new NameValueCollection();
        }

        public PirateXResponseInfo(byte[] headerBytes)
        {
            Headers = HttpUtility.ParseQueryString(Encoding.UTF8.GetString(headerBytes));
        }

        public PirateXResponseInfo(IPirateXPackage responsePackage) 
            :this(responsePackage.HeaderBytes)
        {
            this.ContentBytes = responsePackage.ContentBytes;
        }
    }
    
}
