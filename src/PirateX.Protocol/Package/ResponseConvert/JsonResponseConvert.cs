using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PirateX.Protocol.Package.ResponseConvert
{
    [DisplayColumn("json")]
    public class JsonResponseConvert : IResponseConvert
    {
        public byte[] SerializeObject<T>(T t)
        {
            if (Equals(t, default(T)))
                return null;

            var jsonStr = JsonConvert.SerializeObject(t,JsonSettings);
            return Encoding.UTF8.GetBytes(jsonStr);
        }

        public T DeserializeObject<T>(byte[] datas)
        {
            if (datas == null)
                return default(T);

            var str = Encoding.UTF8.GetString(datas);

            return JsonConvert.DeserializeObject<T>(str);
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


    }

    public class DoubleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var e = (double)value;

            writer.WriteValue(Convert.ToDouble(e.ToString("F4")));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Double) || objectType == typeof(Double?))
                return true;
            return false;
        }
    }
}
