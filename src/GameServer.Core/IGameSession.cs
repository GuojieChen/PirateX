using System;
using GameServer.Core.Package;
using SuperSocket.SocketBase;

namespace GameServer.Core
{
    public interface IGameSession :IAppSession
    {
        bool IsClosed { get; set; }

        long Rid { get; set; }

        DateTime LastResponseTime { get; set; }

        IPackageProcessor PackageProcessor { get; }

        void SendMessage<TResponse>(TResponse message);
        /*
        /// <summary> 额外的操作
        /// </summary>
        /// <param name="obj"></param>
        void ProcessEx(JToken obj);
        */
        int CurrentO { get; set; }

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
        #region 请求返回结果的缓存
        object GetLastResponse(long rid, string c);

        void SetLastReponse(long rid, string c, object o);
        #endregion 

        #region 请求间隔的缓存

        /// <summary> 设置最后一次请求
        /// 如果有时间控制， false表示请求频率太高了
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="c"></param>
        /// <param name="mill"></param>
        bool SetLastRequest(long rid, string c, int mill);

        #endregion

    }
} 
