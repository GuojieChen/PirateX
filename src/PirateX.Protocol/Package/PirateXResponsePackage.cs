namespace PirateX.Protocol.Package
{
    public class PirateXResponsePackage: IPirateXPackage
    {
        public byte[] HeaderBytes { get; set; }
        public byte[] ContentBytes { get; set; }
    }
}
