using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PirateX.Json;

namespace PirateX.Protocol.V1
{
    public class JsonPackage:AbstractProtocolPackag
    {

        public JsonPackage()
        {
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

        public override byte[] SerializeObject<TMessage>(TMessage message)
        {
            return base.Pack(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, JsonSettings)));
        }

        public override IGameRequestInfo DeObject(byte[] datas)
        {
            var body = Encoding.UTF8.GetString(base.Unpack(datas));

            var jObject = JObject.Parse(body);
            
            return new JsonRequestInfo(jObject["C"]?.ToString(), jObject["D"],  Convert.ToBoolean(jObject["R"]), Convert.ToInt32(jObject["O"]));
        }
    }
}
