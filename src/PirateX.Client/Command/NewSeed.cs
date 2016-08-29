using ProtoBuf;

namespace PirateX.Client.Command
{
    public class NewSeed : ExecutorBase<NewSeedResponse>
    {
        public override void Excute(PirateXClient pSocket, NewSeedResponse data)
        {
            var serverKey = new KeyGenerator(data.Seed); 

            pSocket.PackageProcessor.ServerKeys.Add(serverKey.MakeKey());
            pSocket.PackageProcessor.CryptoEnable = true;

        }
    }

    [ProtoContract]
    public class NewSeedResponse
    {
        [ProtoMember(1)]
        public int Seed { get; set; }
    }
}
