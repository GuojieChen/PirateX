using System;
using System.Collections.Generic;
using System.IO;
using PirateX.Client.Crypto;
using PirateX.Client.Zip;

namespace PirateX.Client.Protocol
{
    /// <summary>
    /// 数据包的操作
    /// </summary>
    public static class DataPackage
    {

        public static byte[] Pack(byte[] datas, IList<byte[]> serverKeys, bool seedCreated)
        {
            seedCreated = false; 

            //先压缩 在加密
            var compress = true ? ZipFactory.GetZip().Compress(datas) : datas;

            var d = seedCreated ? CryptoFactory.GetCrypto(1).Encode(compress, serverKeys[0]) : compress;

            var cryptoByte = new byte[1];
            cryptoByte[0] = seedCreated ? (byte)128 : (byte)0;

            var zipByte = new byte[1];
            zipByte[0] = true ? (byte)128 : (byte)0;

            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(d.Length + 6), 0, 4);
                stream.Write(zipByte, 0, 1);
                stream.Write(cryptoByte, 0, 1);
                stream.Write(d, 0, d.Length);

                return stream.ToArray();
            }
        }

        public static byte[] Pack(byte[] datas, IList<byte[]> serverKeys)
        {
            return Pack(datas, serverKeys, false);
        }

        public static byte[] Unpack(byte[] datas, IList<byte[]> clientKeys)
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
                    dataBytes = CryptoFactory.GetCrypto((byte)(7 - i)).Decode(dataBytes, clientKeys[7 - i]);
            }

            if (true)
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
