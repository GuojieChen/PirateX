namespace PirateX.Protocol
{
    public interface IPirateXPackage
    {

        byte[] HeaderBytes { get; set; }

        byte[] ContentBytes { get; set; }
    }
}
