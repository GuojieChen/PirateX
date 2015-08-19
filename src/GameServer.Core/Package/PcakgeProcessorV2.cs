using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Package
{
    public class PcakgeProcessorV2 : IPackageProcessor
    {
        public IZip Zip { get; }
        public ICrypto Crypto { get; }
        public bool ZipEnable { get; set; }
        public bool CryptoEnable { get; set; }
        public IList<byte[]> ClientKeys { get; set; }
        public IList<byte[]> ServerKeys { get; set; }
        public byte[] Pack(byte[] datas)
        {
            throw new NotImplementedException();
        }

        public byte[] Unpack(byte[] datas)
        {
            throw new NotImplementedException();
        }
    }
}
