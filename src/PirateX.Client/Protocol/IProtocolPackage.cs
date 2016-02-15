using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Client.Crypto;
using PirateX.Client.Zip;

namespace PirateX.Client.Protocol
{
    public interface IProtocolPackage
    {/// <summary> 数据压缩操作类
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
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] SerializeObject(ProtocolMessage message);
        /// <summary>
        /// 解析数据包为请求对象
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        IGameRequestInfo DeserializeObject(byte[] datas);
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

    public interface IGameRequestInfo
    {
        /// <summary> 是否是重新请求
        /// </summary>
        bool IsRetry { get; set; }
        /// <summary> 请求序列
        /// </summary>
        int OrderId { get; set; }

        object Body { get; set; }

        T GetTypeBody<T>();
    }
}
