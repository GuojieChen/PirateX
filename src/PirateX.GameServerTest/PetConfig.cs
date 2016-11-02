using PirateX.Core.Config;

namespace PirateX.GameServerTest
{
    [ConfigIndex("AwakeCost", "ElementType")]
    public class PetConfig : IConfigIdEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int AwakeCost { get; set; }

        public int ElementType { get; set; }
    }
}
