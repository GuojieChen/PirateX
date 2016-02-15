using System;
using System.Collections.Generic;
using System.IO;
using PirateX.Client.Crypto;
using PirateX.Client.Zip;

namespace PirateX.Client.Protocol
{
    public abstract class AbstractProtocolPackag : IProtocolPackage

    {
        protected AbstractProtocolPackag(IZip zip,ICrypto crypto)
        {
            ZipEnable = true;
            ClientKeys = new List<byte[]>(8); //用一个字节来表示加密算法的开关，这里用8个长度来存储秘钥
            ServerKeys = new List<byte[]>(8);
            Zip = zip;
            Crypto = crypto;
        }

        protected AbstractProtocolPackag():this(new DefaultZip(),new XXTea())
        {
            
        }

        public IZip Zip { get; private set; }
        public ICrypto Crypto { get; set; }

        public bool ZipEnable { get; set; }
        public bool CryptoEnable { get; set; }
        public IList<byte[]> ClientKeys { get; set; }
        public IList<byte[]> ServerKeys { get; set; }

        public abstract byte[] SerializeObject(ProtocolMessage message);

        public abstract IGameRequestInfo DeserializeObject(byte[] datas);


        public virtual byte[] Pack(byte[] datas)
        {
            var compress = ZipEnable ? Zip.Compress(datas) : datas;

            var d = CryptoEnable ? Crypto.Encode(compress, ServerKeys[0]) : compress;

            var cryptoByte = new byte[1];
            cryptoByte[0] = CryptoEnable ? (byte)128 : (byte)0;

            var zipByte = new byte[1];
            zipByte[0] = ZipEnable ? (byte)128 : (byte)0;

            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(d.Length + 6), 0, 4);
                stream.Write(zipByte, 0, 1);
                stream.Write(cryptoByte, 0, 1);
                stream.Write(d, 0, d.Length);

                return stream.ToArray();
            }
        }

        public virtual byte[] Unpack(byte[] datas)
        {
            byte[] dataBytes = null;
            var zipBit = new byte[1];
            var cryptoBit = new byte[1];

            using (var stream = new MemoryStream(datas))
            {
                var lenBytes = new byte[4];

                stream.Read(lenBytes, 0, 4);
                stream.Read(zipBit, 0, 1);
                stream.Read(cryptoBit, 0, 1);

                var len = BitConverter.ToInt32(lenBytes, 0) - 6;
                dataBytes = new byte[len];
                stream.Read(dataBytes, 0, len);
            }
            for (var i = 0; i < 8; i++)
            {
                var v = GetbitValue(cryptoBit[0], i);
                if (v == 1)
                    dataBytes = Crypto.Decode(dataBytes, ClientKeys[7 - i]);
            }

            if (ZipEnable)
            {
                for (int i = 0; i < 8; i++)
                {
                    var v = GetbitValue(zipBit[0], i);
                    if (v == 1)
                        dataBytes = Zip.Decompress(dataBytes);
                }
            }
            return dataBytes;
        }

        private static int GetbitValue(byte input, int index)
        {
            return (input & ((uint)1 << index)) > 0 ? 1 : 0;
        }
    }
}
