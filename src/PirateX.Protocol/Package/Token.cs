﻿using System;
using ProtoBuf;

namespace PirateX.Protocol
{
    public interface IToken
    {
        /// <summary>
        /// 区号
        /// </summary>
        int Did { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        int Rid { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        int Ts { get; set; }
        /// <summary>
        /// 密钥 需要验证
        /// </summary>
        string Sign { get; set; }
        /// <summary>
        /// 第三方UID
        /// </summary>
        string Uid { get; set; }

        /// <summary>
        /// 角色创建时间的时钟周期表达
        /// </summary>
        long CreateAtUtcTicks { get; set; }
    }
    
    [Serializable]
    [ProtoContract]
    public class Token: IToken
    {
        /// <summary>
        /// 区号
        /// </summary>
        [ProtoMember(1)]
        public int Did { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(2)]
        public int Rid { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        [ProtoMember(3)]
        public int Ts { get; set; }
        /// <summary>
        /// 密钥 需要验证
        /// </summary>
        [ProtoMember(4)]
        public string Sign { get; set; }

        [ProtoMember(5)]
        public string Uid { get; set; }

        /// <summary>
        /// 角色创建时间的时钟周期表达
        /// </summary>
        [ProtoMember(6)]
        public long CreateAtUtcTicks { get; set; }
    }
}
