using System.Collections.Generic;
using PirateX.Protocol.Crypto;
using PirateX.Protocol.Zip;

namespace PirateX.Protocol.Package
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

        IResponseConvert ResponseConvert { get; set; }

        IPirateXRequestPackage UnPackToRequestPackage(byte[] datas);

        byte[] PackRequestPackageToBytes(IPirateXRequestPackage requestPackage);

        byte[] PackResponsePackageToBytes(IPirateXResponsePackage respolnsePackage);

        IPirateXResponsePackage UnPackToResponsePackage(byte[] datas);
    }
}
