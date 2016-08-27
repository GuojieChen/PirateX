using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Protocol.Package
{
    public interface IPirateXResponseInfo
    {
        NameValueCollection Headers { get; set; }

        bool AddHeader(string key, string value);
    }
}
