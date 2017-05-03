using System.Collections;
using PirateX.Protocol;
using ProtoBuf;

namespace PirateX.Client.Command
{
    public class NewSeed : ExecutorBase<NewSeedResponse>
    {
        private static readonly bool[] CryptoByte = new bool[8]
        {
            false, false, false, false,
            false, false, false, true
        };

        public override void Excute(PirateXClient pSocket, NewSeedResponse data)
        {
            var serverKey = new KeyGenerator(data.Seed);

            pSocket.PackageProcessor.UnPackKeys = serverKey.MakeKey();
            pSocket.PackageProcessor.CryptoByte = new BitArray(CryptoByte).ConvertToByte();
        }
    }

    [ProtoContract]
    public class NewSeedResponse
    {
        [ProtoMember(1)]
        public int Seed { get; set; }
    }
}
