namespace PirateX.Client.Crypto
{
    public class NoneCrypto : ICrypto
    {
        public byte[] Encode(byte[] data,byte[] serverKey)
        {
            return data; 
        }

        public byte[] Decode(byte[] datas,byte[] clientKey)
        {
            return datas; 
        }
    }
}
