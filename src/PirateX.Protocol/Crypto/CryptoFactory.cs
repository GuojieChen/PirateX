namespace PirateX.Protocol.Crypto
{
    public static class CryptoFactory
    {
        private readonly static ICrypto Default = new NoneCrypto();

		private readonly static ICrypto XXTEA = new XXTea() ; 
        
        public static ICrypto GetCrypto(byte index)
        {
            return XXTEA; 
        }
    }
}
