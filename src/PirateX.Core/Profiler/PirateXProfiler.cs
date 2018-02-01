using System;

namespace PirateX.Core
{
    public class PirateXProfiler
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "@timestamp")]
        public DateTime Timestamp { get; set; }

        public string Ip { get; set; }
        
        public string SessionId { get; set; }


        public long Rid { get; set; }

        public int Did { get; set; }
    }
}
