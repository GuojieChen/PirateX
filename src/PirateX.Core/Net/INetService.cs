using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol.Package;

namespace PirateX.Core.Net
{
    /// <summary>
    /// 外部请求分发给内部逻辑服务器
    /// </summary>
    public interface INetService
    {
        void Setup(INetManager netManager);
        /// <summary>
        /// 收到客户端请求，处理并获取返回结果
        /// </summary>
        /// <param name="protocolPackage"></param>
        /// <param name="body"></param>
        byte[] ProcessRequest(IProtocolPackage protocolPackage, byte[] body);

        void Ping(int onlinecount);

        void OnSessionClosed(IProtocolPackage protocolPackage);

        void Start();

        void Stop();
    }
}
