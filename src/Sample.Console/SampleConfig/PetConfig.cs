using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Config;

namespace GameServer.Console.SampleConfig
{
    public class PetConfig : IConfigEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
