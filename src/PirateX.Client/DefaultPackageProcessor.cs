using System;
using System.Collections.Generic;
using System.IO;
using PirateX.Client.Crypto;
using PirateX.Client.Zip;

namespace PirateX.Client
{
    public class DefaultPackageProcessor:IPackageProcessor
    {
        public bool ZipEnable { get; set; }
        public bool CryptoEnable { get; set; }
        public IList<byte[]> ClientKeys { get; set; }
        public IList<byte[]> ServerKeys { get; set; }

        public DefaultPackageProcessor()
        {
            ZipEnable = true;
            ClientKeys = new List<byte[]>(8); //用一个字节来表示加密算法的开关，这里用8个长度来存储秘钥
            ServerKeys = new List<byte[]>(8);
        }

        public byte[] Pack(byte[] datas)
        {
            var compress = ZipEnable ? ZipFactory.GetZip().Compress(datas) : datas;

            var d = CryptoEnable ? CryptoFactory.GetCrypto(1).Encode(compress, ServerKeys[0]) : compress;

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
                var v = Utils.GetbitValue(cryptoBit[0], i);
                if (v == 1)
                    dataBytes = CryptoFactory.GetCrypto((byte)(7 - i)).Decode(dataBytes, ClientKeys[7 - i]);
            }

            if (ZipEnable)
            {
                for (int i = 0; i < 8; i++)
                {
                    var v = Utils.GetbitValue(zipBit[0], i);
                    if (v == 1)
                        dataBytes = ZipFactory.GetZip().Decompress(dataBytes);
                }
            }
            return dataBytes;
        }
    }
}
