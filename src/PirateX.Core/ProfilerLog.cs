using System;
using Newtonsoft.Json;
using NLog;

namespace PirateX.Core
{
    public class ProfilerLog
    {
        private static readonly Logger ProfilerLogger = LogManager.GetLogger("_Profiler_");

        public string Token { get; set; }

        public string Ip { get; set; }

        [JsonProperty("@timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// tin --> itin
        /// </summary>
        public Ticks Tin { get; set; }

        /// <summary>
        /// itin --> itout
        /// </summary>
        public Ticks iTin { get; set; }

        /// <summary>
        /// itout-->tout
        /// </summary>
        public Ticks Tout { get; set; }


        public Ticks TExcute { get; set; }

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
            if(ProfilerLogger.IsInfoEnabled)
                ProfilerLogger.Info(this.ToString);
        }
    }


    public class Ticks
    {
        public long Start { get; set; }

        public long End { get; set; }

        public long Duration { get; set; }
    }
}
