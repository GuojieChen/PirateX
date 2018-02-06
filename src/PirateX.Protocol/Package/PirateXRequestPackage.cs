namespace PirateX.Protocol
{
    public class PirateXRequestPackage: IPirateXPackage
    {
        public byte[] HeaderBytes { get; set; }
        public byte[] ContentBytes { get; set; }
    }
}
