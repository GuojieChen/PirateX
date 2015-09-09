using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Broadcas
{
    public interface IMsgBroadcast
    {
        /// <summary> 广播消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="rids"></param>
        void Send<T>(T msg, params long[] rids);
        /// <summary> 向一个游戏服推送信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="districtId"></param>
        void SendToDistrict<T>(T msg, params int[] districtId);
    }
}
