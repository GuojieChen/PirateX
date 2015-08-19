using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GameServer.Core.Protocol.V1
{
    public class JsonRequestInfo :IGameRequestInfo
    {
        public string Key { get; }
        public bool IsRetry { get; set; }
        public int OrderId { get; set; }
        public object Body { get; set; }

        public JsonRequestInfo(string key, object body,bool r,int orderId)
        {
            Key = key; 
            IsRetry = r;
            OrderId = orderId;
            Body = body;
        }


        public T GetTypeBody<T>()
        {
            if (Body == null)
                return default(T);

            return (T) ((JToken) Body).ToObject(typeof (T));
        }
    }
}
