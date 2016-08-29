using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PirateX.Protocol.Package
{
    public class PirateXRequestInfo : IPirateXRequestInfoBase
    {
        public string C { get; set; }
        public int O { get; set; }

        public bool R { get; set; }

        public long Timestamp { get; set; }

        public string Token { get; set; }

        public NameValueCollection Headers { get; set; }

        public NameValueCollection QueryString { get; set; }

        public IPirateXRequestPackage ToRequestPackage()
        {
            return new PirateXRequestPackage()
            {
                HeaderBytes = Encoding.UTF8.GetBytes($"{String.Join("&", Headers.AllKeys.Select(a => a + "=" + Headers[a]))}"),
                ContentBytes = Encoding.UTF8.GetBytes($"{String.Join("&", QueryString.AllKeys.Select(a => a + "=" + QueryString[a]))}"),
            };
        }

        public string Key { get; set; }

        public PirateXRequestInfo(NameValueCollection headers,NameValueCollection queryString)
        {
            this.Headers = headers;
            this.QueryString = queryString;
            this.C = this.Key = headers["c"];
            this.O = Convert.ToInt32(headers["o"]);
            this.R = Convert.ToBoolean(headers["r"]);
            this.Timestamp = Convert.ToInt64(headers["t"]);
            this.Token = Convert.ToString(headers["token"]);
        }

        public PirateXRequestInfo(byte[] headerBytes, byte[] contentBytes)
            :this(HttpUtility.ParseQueryString(Encoding.UTF8.GetString(headerBytes)), HttpUtility.ParseQueryString(Encoding.UTF8.GetString(contentBytes)) )
        {
            
        }

        public PirateXRequestInfo(IPirateXRequestPackage requestPackage):
            this(requestPackage.HeaderBytes,requestPackage.ContentBytes)
        {
            
        }
    }
}
