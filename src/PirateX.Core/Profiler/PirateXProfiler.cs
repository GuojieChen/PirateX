using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Profiler
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
