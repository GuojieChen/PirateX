namespace PirateX.Protocol
{
    public static class CryptoFactory
    {
        private static readonly ICrypto Default = new NoneCrypto();

        private static readonly ICrypto XXTEA = new XXTea();

        public static ICrypto GetCrypto(byte index)
        {
            switch (index)
            {
                case 7:
                    return XXTEA;

                default:
                    return null;
            }
        }
    }
}
