using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol.Package;

namespace PirateX.Core.Net
{
    public interface INetService
    {
        void Setup(INetManager netManager);
        /// <summary>
        /// 收到客户端请求，交由Actor进行处理
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
