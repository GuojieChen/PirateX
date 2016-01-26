using System;

namespace PirateX.Client.Crypto
{
    public class CryptoAttribute:Attribute
    {
        public byte Index { get; private set; }

        public CryptoAttribute(byte index)
        {
            Index = index;
        }
    }
}
