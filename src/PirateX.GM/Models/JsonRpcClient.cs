using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using AustinHarris.JsonRpc;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Diagnostics;

namespace PirateX.GM.Models
{
    public class JsonRpcClient
    {
        private static object idLock = new object();
        private static int id = 0;
        public Uri ServiceEndpoint = null;
        private static int myId = 0;


        public JsonRpcClient(Uri serviceEndpoint)
        {
            ServiceEndpoint = serviceEndpoint;
        }

        private static Stream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }

        public Action<Exception> OnError;

        public T Invoke<T>(string method,params object[] arg)
        {
            JsonRequest jsonRpc = new JsonRequest() { Method = method,Params = arg };

            WebRequest req = null;
            JsonResponse<T> rjson = null;
            try
            {

                myId++;
                jsonRpc.Id = myId.ToString();
                req = HttpWebRequest.Create(new Uri(ServiceEndpoint, "?callid=" + myId.ToString()));
                req.Method = "Post";
                req.ContentType = "application/json-rpc";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var stream = new StreamWriter(req.GetRequestStream());
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpc);
            stream.Write(json);

            stream.Close();

            string sstream = "";
            try
            {
                var resp = (HttpWebResponse)req.GetResponse();

                using (var rstream = new StreamReader(CopyAndClose(resp.GetResponseStream())))
                {
                    sstream = rstream.ReadToEnd();
                }

                rjson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonResponse<T>>(sstream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }

            if (rjson == null)
            {
                if (!string.IsNullOrEmpty(sstream))
                {
                    JObject jo = Newtonsoft.Json.JsonConvert.DeserializeObject(sstream) as JObject;
                    OnError?.Invoke(new Exception(jo["Error"].ToString()));
                }
                else
                {
                    OnError?.Invoke(new Exception("Empty response"));
                }
            }

            if (rjson == null)
                return default(T);

            return rjson.Result;
        }

    }
}