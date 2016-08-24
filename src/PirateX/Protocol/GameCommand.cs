using System;
using System.Diagnostics;
using PirateX.Core;
using SuperSocket.SocketBase;

namespace PirateX.Protocol
{

    /// <summary>
    /// GameCommand
    /// </summary>
    /// <typeparam name="TSession">The type of the socket session.</typeparam>
    /// <typeparam name="TRequest">The type of the request info.</typeparam>
    /// <typeparam name="TResponse">The type of the response info.</typeparam>
    public abstract class GameCommand<TSession, TRequest, TResponse> : GameCommandBase<TSession, TRequest>
        where TSession : IGameSession, IAppSession<TSession, IGameRequestInfo>, new()
    {
        protected override void ExecuteGameCommand(TSession session, TRequest data)
        {
            var sw = Stopwatch.StartNew();
            long pms = 0; //逻辑处理耗时
            long sms = 0; //包括发送处理耗时
            long ms = 0; //总共处理耗时
            var start = DateTime.Now;

            var cacheName = Convert.ToString(session.CurrentO);
            var appserver = (IGameServer) session.AppServer;

            appserver.StartRequest(session, cacheName);

            var response = ExecuteResponseCommand(session, data);
            pms = sw.ElapsedMilliseconds;

            if (!Equals(response, default(TResponse)))
            {
                SendResponse(session, response);

                //if (!IgnoreCmds.Contains(cacheName))
                appserver.EndRequest(session, cacheName, response);
                sms = sw.ElapsedMilliseconds;
            }
            else
            {//返回默认的给客户端
                SendResponse(session, null);

                //if (!IgnoreCmds.Contains(cacheName))
                appserver.EndRequest(session, cacheName, null);
                sms = sw.ElapsedMilliseconds;
            }

            if (OnResponsedEvent != null)
                OnResponsedEvent(session, data);

            // 小红点的问题
            if (NewsOverEvent != null && NewsCloseEvent != null)
            {
                var key = NewsOverEvent();
                if (!string.IsNullOrEmpty(key))
                    NewsCloseEvent(session, key);
            }

            session.LastResponseTime = DateTime.Now;
            ms = sw.ElapsedMilliseconds;
            session.ProcessedRequest(Name, Args, pms, sms, ms, start, DateTime.Now, cacheName);
        }

        /// <summary>
        /// Executes the json command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="data">The command info.</param>
        protected abstract TResponse ExecuteResponseCommand(TSession session, TRequest data);

        protected void SendResponse(TSession session, TResponse response)
        {
            session.SendMessage(new ProtocolMessage
            {
                C = Name,
                D = response,
                O = session.CurrentO
            });
        }


        //客户端请求失败 
        /*
          客户端请求失败 尝试重新请求
          服务端查看请求列表中是否有该请求
          有    则等待
          没有  则查看Response是否有
        */
        protected override bool Retry(TSession session, string cacheName)
        {
            var appserver = (IGameServer)session.AppServer;

            if (appserver.ExistsReqeust(session, cacheName))
            {
                // 已有请求 等待完成
                var response = appserver.GetResponse<TResponse>(session, cacheName);
                if (Equals(default(TResponse), response))
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Retry fail,Session [{session.SessionID}],cacheName : {cacheName}");
                    return true;
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Retry success,Session [{session.SessionID}],cacheName : {cacheName}");

                    session.SendMessage(new ProtocolMessage
                    {
                        C = Name,
                        D = response, 
                        O = session.CurrentO
                    });

                    return true;
                }
            }
            else
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"Retry no request,Session [{session.SessionID}],cacheName : {cacheName}");
            }
            return false;
        }


        public delegate void OnResponsedEventHandler(TSession session, TRequest request);
        public OnResponsedEventHandler OnResponsedEvent;

        protected delegate string NewsOverEventHandler();
        /// <summary> 在已经没有消息内容的情况下 返回 NewsKey 关键值，这样后续可以自动去关闭掉红点
        /// </summary>
        protected NewsOverEventHandler NewsOverEvent;

        protected delegate void NewsCloseEventHandler(IAppSession session, string key);

        protected NewsCloseEventHandler NewsCloseEvent;
    }
}
