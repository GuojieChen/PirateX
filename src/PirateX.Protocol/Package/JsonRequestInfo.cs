//using System.Collections.Specialized;
//using Newtonsoft.Json.Linq;

//namespace PirateX.Protocol.Package
//{
//    public class JsonRequestInfo :IPirateXRequestInfo
//    {
//        public string Key { get; }
//        public bool R { get; set; }
//        public long Timestamp { get; set; }
//        public NameValueCollection Headers { get; set; }
//        public NameValueCollection QueryString { get; set; }
//        public int O { get; set; }
//        public object Body { get; set; }

//        public JsonRequestInfo(string key, object body,bool r,int orderId)
//        {
//            Key = key; 
//            R = r;
//            O = orderId;
//            Body = body;
//        }


//        public T GetTypeBody<T>()
//        {
//            if (Body == null)
//                return default(T);

//            return (T) ((JToken) Body).ToObject(typeof (T));
//        }
//    }
//}
