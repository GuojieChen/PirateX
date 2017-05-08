﻿using PirateX.Protocol.Package;

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

        public string SessionId { get; set; }

        public byte[] ClientKeys { get; set; }

        public byte[] ServerKeys { get; set; }

        public byte CryptoByte { get; set; }

        public string ResponseCovnert { get; set; }

        public IToken Token { get; set; }

        public IPirateXRequestInfoBase Request { get; set; }

        public string RemoteIp { get; set; }

        /// <summary>
        /// 客户端请求自增序列号
        /// </summary>
        public int LastNo { get; set; }
    }
}
