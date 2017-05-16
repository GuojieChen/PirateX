using System.Collections.Generic;
using System.Net;
using PirateX.Protocol.Crypto;
using PirateX.Protocol.Zip;

namespace PirateX.Protocol.Package
{
    public interface IProtocolPackage
    {
        string Id { get; }

        int Rid { get; set; }

        /// <summary> 数据压缩操作类
        /// </summary>
        IZip Zip { get; }
        /// <summary> 压缩是否启用
        /// </summary>
        bool ZipEnable { get; set; }
        /// <summary> 客户端秘钥列表
        /// </summary>
        byte[] PackKeys { get; set; }
        /// <summary> 服务端秘钥列表
        /// </summary>
        byte[] UnPackKeys { get; set; }
        byte CryptoByte { get; set; }

        int LastNo { get; set; }

        EndPoint RemoteEndPoint { get; set; }

        //IResponseConvert ResponseConvert { get; set; }

        //IPirateXRequestPackage UnPackToRequestPackage(byte[] datas);

        byte[] PackPacketToBytes(IPirateXPackage requestPackage);

        //byte[] PackResponsePackageToBytes(IPirateXResponsePackage respolnsePackage);

        IPirateXPackage UnPackToPacket(byte[] datas);

        void Send(byte[] body);

        void Close();
    }
}
