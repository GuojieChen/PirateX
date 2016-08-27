using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using PirateX.Protocol.Crypto;
using PirateX.Protocol.Package;
using PirateX.Protocol.Zip;

namespace PirateX.Protocol
{
    public interface IProtocolPackage
    {
        /// <summary> 数据压缩操作类
        /// </summary>
        IZip Zip { get; }
        /// <summary> 数据加密解密操作类
        /// </summary>
        ICrypto Crypto { get; }

        /// <summary> 压缩是否启用
        /// </summary>
        bool ZipEnable { get; set; }
        /// <summary> 加密是否启用
        /// </summary>
        bool CryptoEnable { get; set; }
        /// <summary> 客户端秘钥列表
        /// </summary>
        IList<byte[]> ClientKeys { get; set; }
        /// <summary> 服务端秘钥列表
        /// </summary>
        IList<byte[]> ServerKeys { get; set; }
        
        byte[] Pack(byte[] datas);
        byte[] Unpack(byte[] datas);
    }

    public class ProtocolMessage
    {
        public string B { get; set; }
        public string C { get; set; }

        public object D { get; set; }

        public object Code { get; set; }
        
        public string Msg { get; set; }

        public int O { get; set; }
    }
}
