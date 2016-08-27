using System;
using SuperSocket.SocketBase;

namespace PirateX.Protocol
{
    public interface IGameSessionBase :IAppSession
    {
        /// <summary> 是否已经登录
        /// </summary>
        bool IsLogin { get; set; }

        bool IsClosed { get; set; }
        /// <summary> 角色ID
        /// </summary>
        long Rid { get; set; }
        /// <summary> 最后请求处理时间 UTC
        /// </summary>
        DateTime LastResponseTime { get; set; }

        /// <summary> 当前请求序列
        /// </summary>
        int CurrentO { get; set; }
        /// <summary> 最后请求序列
        /// </summary>
        int MyLastO { get; set; }

        IProtocolPackage ProtocolPackage { get; set; } 
    }
} 
