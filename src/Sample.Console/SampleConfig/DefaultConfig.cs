using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Config;

namespace GameServer.Console.SampleConfig
{
    public class DefaultConfig : IConfigKeyValueEntity
    {
        public string Id { get; set; }

        public string V { get; set; }
    }
}
