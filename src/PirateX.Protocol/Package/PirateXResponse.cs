using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Protocol.Package
{
    public class PirateXResponse :IPirateXResponseInfo
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

        public PirateXResponse()
        {
            Headers = new NameValueCollection();
        }
    }
}
