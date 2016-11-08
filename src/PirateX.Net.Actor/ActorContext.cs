using PirateX.Protocol.Package;

namespace PirateX.Net.Actor
{
    public class ActorContext
    {
        public byte Version { get; set; }

        public string SessionId { get; set; }

        public byte[] ClientKeys { get; set; }

        public byte[] ServerKeys { get; set; }

        public IPirateXRequestInfoBase Request { get; set; }
    }
}
