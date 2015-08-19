using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Json;
using GameServer.Core.Package;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace GameServer.Core.Protocol.V1
{
    public class JsonSerialize:IProtocolPackage<JsonRequestInfo>
    {
        public JsonSerialize(IPackageProcessor packageProcessor)
        {
            this.PackageProcessor = packageProcessor; 
        }


        /// <summary>
        /// JSON 序列化配置
        /// </summary>
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                var timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                };

                settings.Converters.Add(timeConverter);
                settings.Converters.Add(new DoubleConverter());

                return settings;
            }
        }

        public IPackageProcessor PackageProcessor { get; set; }

        public byte[] SerializeObject<TMessage>(TMessage message)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, JsonSettings));
        }

        public JsonRequestInfo DeObject(byte[] datas)
        {
            var body = Encoding.UTF8.GetString(PackageProcessor.Unpack(datas));

            var jObject = JObject.Parse(body);
            
            return new JsonRequestInfo(jObject["C"]?.ToString(), jObject["D"],  Convert.ToBoolean(jObject["R"]), Convert.ToInt32(jObject["O"]));
        }
    }
}
