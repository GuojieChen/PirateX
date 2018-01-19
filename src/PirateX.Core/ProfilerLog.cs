using System;
using Newtonsoft.Json;
using NLog;

namespace PirateX.Core
{
    public class ProfilerLog
    {
        public static readonly Logger ProfilerLogger = LogManager.GetLogger("_PirateX_");

        public string Token { get; set; }

        public string Ip { get; set; }

        [JsonProperty("@timestamp")]
        public DateTime Timestamp => StartAt; 

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public double Milliseconds => EndAt.Subtract(StartAt).TotalMilliseconds;

        public string C { get; set; }

        public long Tin { get; set; }
        public long iTin { get; set; }
        public long Tout { get; set; }

        private JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, _settings);
        }

        public void Log()
        {
            if (ProfilerLogger.IsInfoEnabled)
                ProfilerLogger.Info(this.ToString);
        }
    }
}
