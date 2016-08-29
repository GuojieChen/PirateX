using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;

namespace PirateX.Client
{
    public abstract class JsonExecutorBase<TSocketClient, TResponseInfo> : IJsonExecutor<TSocketClient, TResponseInfo>
        where TSocketClient:PirateXClient
    {
        public TResponseInfo GetResponseInfo(byte[] data)
        {
            return JsonConvert.DeserializeObject<TResponseInfo>(Encoding.UTF8.GetString(data)); 
        }

        public abstract void Excute(TSocketClient client, TResponseInfo data);
        public NameValueCollection Header { get; set; }
    }
}
