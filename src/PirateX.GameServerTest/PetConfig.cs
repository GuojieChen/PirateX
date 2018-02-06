using System;
using PirateX.Core;

namespace PirateX.GameServerTest
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
