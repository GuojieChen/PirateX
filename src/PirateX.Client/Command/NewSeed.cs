﻿namespace PirateX.Client.Command
{
    public class NewSeed : JsonExecutorBase<PSocketClient,NewSeedResponse>
    {
        public override void Excute(PSocketClient pSocket, NewSeedResponse data)
        {
            if (data.Seed == null)
            {
                pSocket.Close();
                return;
            }

            var serverKey = new KeyGenerator(data.Seed.Value); 

            pSocket.PackageProcessor.ServerKeys.Add(serverKey.MakeKey());
            pSocket.PackageProcessor.CryptoEnable = true;

        }
    }

    public class NewSeedResponse
    {
        public int? Seed { get; set; }
    }
}
