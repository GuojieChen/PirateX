using System;
using System.Diagnostics;
using System.Linq;
using SuperSocket.SocketBase;

namespace GameServer.Core.PkProtocol
{
    public abstract class JsonSubCommand<TRequest, TResponse> : JsonSubCommand<PSession, TRequest, TResponse>
    {

    }

    /// <summary>
    /// JsonSubCommand
    /// </summary>
    /// <typeparam name="TSession">The type of the socket session.</typeparam>
    /// <typeparam name="TRequest">The type of the json command info.</typeparam>
    /// <typeparam name="TResponse">The type of the response info.</typeparam>
    public abstract class JsonSubCommand<TSession, TRequest, TResponse> : JsonSubCommandBase<TSession, TRequest>
        where TSession : IGameSession, IAppSession<TSession, ISocketRequestInfo>, new()
    {
        //protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static string[] IgnoreCmds = new[] { "Pluse", "NewSeed", "RoleLoginV2", "Ping", "Pluse" };

        protected override void ExecuteJsonCommand(TSession session, TRequest data)
        {
            var sw = Stopwatch.StartNew();
            long pms = 0; //逻辑处理耗时
            long sms = 0; //包括发送处理耗时
            long ms = 0; //总共处理耗时
            var start = DateTime.Now;

            var cacheName = Convert.ToString(session.CurrentO);
            var rid = session.Items.ContainsKey("Rid") ? Convert.ToInt64(session.Items["Rid"]) : 0;

            var response = ExecuteResponseCommand(session, data);
            pms = sw.ElapsedMilliseconds;

            if (!Equals(response, default(TResponse)))
            {
                SendResponse(session, response);
                if (!IgnoreCmds.Contains(cacheName))
                    session.SetLastReponse(rid, cacheName, response);
                sms = sw.ElapsedMilliseconds;
            }
            else
            {//返回默认的给客户端
                SendResponse(session, null);
                if (!IgnoreCmds.Contains(cacheName))
                    session.SetLastReponse(rid, cacheName, null);
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
            session.SendMessage(new
            {
                C = Name,
                D = response,
                O = session.CurrentO
            });
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
