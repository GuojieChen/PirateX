using System;

namespace PirateX.Protocol.Crypto
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
