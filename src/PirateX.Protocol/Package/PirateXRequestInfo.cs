using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Protocol.Package
{
    public class PirateXRequestInfo : IPirateXRequestInfo
    {
        public int O { get; set; }

        public bool R { get; set; }

        public long Timestamp { get; set; }

        public string Token { get; set; }

        public NameValueCollection Headers { get; set; }

        public NameValueCollection QueryString { get; set; }

        public string Key { get; set; }

        public PirateXRequestInfo(string key, NameValueCollection headers,NameValueCollection queryString)
        {
            this.Key = key;
            this.Headers = headers;
            this.QueryString = queryString;

            this.O = Convert.ToInt32(headers["O"]);
            this.R = Convert.ToBoolean(headers["R"]);
            this.Timestamp = Convert.ToInt64(headers["T"]);
            this.Token = Convert.ToString(headers["token"]);
        }
    }
}
