using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Protocol.Package;

namespace GameServer.Core.Protocol
{
    public interface IProtocolPackage<out TRequestInfo>
        where TRequestInfo :IGameRequestInfo
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

        /// <summary>
        /// 序列化包为二进制数据
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] SerializeObject<TMessage>(TMessage message);

        /// <summary>
        /// 解析数据包为请求对象
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        TRequestInfo DeObject(byte[] datas); 
    }

    public interface IProtocolPackage : IProtocolPackage<IGameRequestInfo>
    {
        
    }
}
