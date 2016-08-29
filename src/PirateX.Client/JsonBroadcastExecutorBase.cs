using System.Collections.Specialized;
using Newtonsoft.Json;

namespace PirateX.Client
{
    public abstract class JsonBroadcastExecutorBase <TSocketClient,TData> : IJsonBroadcastExecutor<TSocketClient,TData>
        where TSocketClient : PirateXClient
    {
        public TData GetData(string data)
        {
            return JsonConvert.DeserializeObject<TData>(data); 
        }

        public abstract void Execute(TSocketClient client, TData data);
        public NameValueCollection Header { get; set; }
    }
}
