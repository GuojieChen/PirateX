using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Service
{
    public interface IGameServer 
    {
        IServerContainer ServerContainer { get; set; }
        /// <summary> 推送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        void Push<T>(T message) where T:IPushMessage;
    }
    /// <summary>
    /// 跪送消息模型
    /// </summary>
    public interface IPushMessage
    {
        string Token { get; set; }
        string Channel { get; set; }
        string LogTime { get; set; }
        string Sound { get; set; }
    }
}
