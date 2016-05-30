using System;
using Autofac;
using PirateX.Protocol;
using SuperSocket.SocketBase;

namespace PirateX
{
    public interface IGameSession :IGameSessionBase
    {
        void SendMessage(ProtocolMessage message);

        ILifetimeScope Reslover { get; set; }

        /*
        /// <summary> 额外的操作
        /// </summary>
        /// <param name="obj"></param>
        void ProcessEx(JToken obj);
        */

        /// <summary> 当前请求序列
        /// </summary>
        int CurrentO { get; set; }
        /// <summary> 最后请求序列
        /// </summary>
        int MyLastO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">请求参数</param>
        /// <param name="pms">逻辑处理耗时</param>
        /// <param name="sms">加上发送的处理耗时</param>
        /// <param name="ms">总共耗时</param>
        /// <param name="name">请求方法名</param>
        /// <param name="start">请求接收时间（服务器本地时间）</param>
        /// <param name="end">请求处理结束时间（服务器本地时间）</param>
        void ProcessedRequest(string name,object args, long pms, long sms, long ms,DateTime start,DateTime end,string o);
        
    }
} 
