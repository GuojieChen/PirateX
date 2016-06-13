using System;
using Newtonsoft.Json;

namespace PirateX.Protocol.Package
{
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
