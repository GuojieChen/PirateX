using System.Collections.Generic;

namespace GameServer.Core.Package
{
    /// <summary>
    /// 包处理器接口
    /// </summary>
    public interface IPackageProcessor
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
        /// <summary> 数据打包过程
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        byte[] Pack(byte[] datas);
        /// <summary> 数据解包过程
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        byte[] Unpack(byte[] datas); 
    }
}
