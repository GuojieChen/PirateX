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

        public IDictionary<string, string> Headers { get; set; }

        public IDictionary<string, string> QueryString { get; set; }

        public IPirateXRequestPackage ToRequestPackage()
        {
            return new PirateXRequestPackage()
            {
                HeaderBytes = Encoding.UTF8.GetBytes($"{String.Join("&", Headers.Keys.Select(a => a + "=" + Headers[a]))}"),
                ContentBytes = Encoding.UTF8.GetBytes($"{String.Join("&", QueryString.Keys.Select(a => a + "=" + QueryString[a]))}"),
            };
        }

        public string Key { get; set; }

        public PirateXRequestInfo(IDictionary<string, string> headers, IDictionary<string, string> queryString)
        {
            this.Headers = headers;
            this.QueryString = queryString;
            if (headers.ContainsKey("c"))
                this.C = this.Key =  headers["c"];
            if (headers.ContainsKey("o"))
                this.O = Convert.ToInt32(headers["o"]);
            if(headers.ContainsKey("r"))
                this.R = Convert.ToBoolean(headers["r"]);
            if (headers.ContainsKey("t"))
                this.Timestamp = Convert.ToInt64(headers["t"]);
            if (headers.ContainsKey("token"))
                this.Token = Convert.ToString(headers["token"]);
        }

        public PirateXRequestInfo(string headerStr, string contentStr)
            :this(headerStr.ToQueryDic(), contentStr.ToQueryDic())
        {
            
        }

        public PirateXRequestInfo(byte[] headerBytes, byte[] contentBytes)
            :this(Encoding.UTF8.GetString(headerBytes), Encoding.UTF8.GetString(contentBytes) )
        {
            
        }

        public PirateXRequestInfo(IPirateXRequestPackage requestPackage):
            this(requestPackage.HeaderBytes,requestPackage.ContentBytes)
        {
            
        }
        
    }
}
