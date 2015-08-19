using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Protocol
{
    /// <summary> 协议抽象
    /// </summary>
    public interface IGameRequestInfo : IRequestInfo
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
