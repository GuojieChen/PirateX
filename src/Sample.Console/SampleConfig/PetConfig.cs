using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core;

namespace GameServer.Console.SampleConfig
{
    [ConfigIndex("AwakeCost", "ElementType")]
    public class PetConfig : IConfigIdEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AwakeCost { get; set; }

        public int ElementType { get; set; }
    }
}
