using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using PirateX.Protocol.Package;

namespace PirateX.Client
{
    public abstract class ExecutorBase<TResponseInfo> : IPirateXClientExecutor
    {
        public TResponseInfo GetResponseInfo(byte[] data)
        {
            return JsonConvert.DeserializeObject<TResponseInfo>(Encoding.UTF8.GetString(data)); 
        }

        public abstract void Excute(PirateXClient client, TResponseInfo data);
        public NameValueCollection Header { get; set; }
        public IResponseConvert ResponseConvert { get; set; }
        public void Excute(PirateXClient client, byte[] data)
        {
            var t = ResponseConvert.DeserializeObject<TResponseInfo>(data);

            Excute(client, t);
        }
    }
}
