using System;
using System.Collections.Generic;
using System.IO;

namespace GameServer.Core.Package
{
    /// <summary> 用于数据转送的处理器
    /// </summary>
    public class TransitPackageProcessor:IPackageProcessor
    {
        public IZip Zip { get; }
        public ICrypto Crypto { get; }

        public bool ZipEnable { get; set; }
        public bool CryptoEnable { get; set; }
        public IList<byte[]> ClientKeys { get; set; }
        public IList<byte[]> ServerKeys { get; set; }

        public byte[] Pack(byte[] datas)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(datas.Length + 4), 0, 4);
                stream.Write(datas, 0, datas.Length);

                return stream.ToArray();
            }
        }

        public byte[] Unpack(byte[] datas)
        {
            byte[] dataBytes = null;

            using (var stream = new MemoryStream(datas))
            {
                var lenBytes = new byte[4];

                stream.Read(lenBytes, 0, 4);

                var len = BitConverter.ToInt32(lenBytes, 0) - 4;
                dataBytes = new byte[len];
                stream.Read(dataBytes, 0, len);
            }
            
            return dataBytes;
        }
    }
}
