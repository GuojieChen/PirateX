namespace PirateX.Protocol.Package
{
    public class PirateXRequestPackage:IPirateXRequestPackage
    {
        public byte[] HeaderBytes { get; set; }
        public byte[] ContentBytes { get; set; }

    }
}
