using PirateX.Protocol;
using ProtoBuf;

namespace PirateX.Client.Command
{
    public class NewSeed : ExecutorBase<NewSeedResponse>
    {
        private static readonly byte[] CryptoByte = new byte[8]{1,0,0,0,0,0,0,0};

        public override void Excute(PirateXClient pSocket, NewSeedResponse data)
        {
            var serverKey = new KeyGenerator(data.Seed); 

            pSocket.PackageProcessor.UnPackKeys = serverKey.MakeKey();
            pSocket.PackageProcessor.CryptoByte = CryptoByte;
        }
    }

    [ProtoContract]
    public class NewSeedResponse
    {
        [ProtoMember(1)]
        public int Seed { get; set; }
    }
}
