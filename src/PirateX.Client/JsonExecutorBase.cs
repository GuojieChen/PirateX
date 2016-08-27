using System.Collections.Specialized;
using Newtonsoft.Json;

namespace PirateX.Client
{
    public abstract class JsonExecutorBase<TSocketClient, TResponseInfo> : IJsonExecutor<TSocketClient, TResponseInfo>
        where TSocketClient:PSocketClient
    {
        public TResponseInfo GetResponseInfo(string data)
        {
            return JsonConvert.DeserializeObject<TResponseInfo>(data); 
        }

        public abstract void Excute(TSocketClient client, TResponseInfo data);
        public NameValueCollection Header { get; set; }
    }
}
