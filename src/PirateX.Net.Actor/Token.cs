using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace PirateX.Net.Actor
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
        long Rid { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        long Ts { get; set; }
        /// <summary>
        /// 密钥 需要验证
        /// </summary>
        string Sign { get; set; }
        /// <summary>
        /// 第三方UID
        /// </summary>
        string Uid { get; set; }
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
        public long Rid { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        [ProtoMember(3)]
        public long Ts { get; set; }
        /// <summary>
        /// 密钥 需要验证
        /// </summary>
        [ProtoMember(4)]
        public string Sign { get; set; }

        [ProtoMember(6)]
        public string Uid { get; set; }
    }
}
