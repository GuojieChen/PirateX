using System.Collections.Generic;
using PirateX.Protocol.Package;

namespace PirateX.Core.Actor
{
    public class ActorContext
    {
        public byte Version { get; set; }
        /// <summary>
        /// 动作
        /// 1 请求
        /// 2 断线
        /// </summary>
        public byte Action { get; set; }

        public string ResponseCovnert { get; set; }

        public string SessionId { get; set; }

        public IToken Token { get; set; }

        public IPirateXRequestInfoBase Request { get; set; }

        public string RemoteIp { get; set; }

        /// <summary>
        /// 客户端请求自增序列号
        /// </summary>
        public int LastNo { get; set; }

        public Dictionary<string ,string> Profile { get; set; }

        public string ServerName { get; set; }

        public int OnlineCount { get; set; }

        public Dictionary<string,string> ServerItmes { get; set; }
        /// <summary>
        /// 内部网络地址
        /// </summary>
        public string SocketAddress { get; set; }
    }
}
