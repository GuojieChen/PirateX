using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Config;
using ServiceStack.DataAnnotations;

namespace GameServer.Console.SampleConfig
{
    public class DefaultConfig : IConfigKeyValueEntity
    {
        public string Id { get; set; }

        [Alias("Value")]
        public string V { get; set; }
    }
}
