using System;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Logging;

namespace PirateX.Protocol
{
    /// <summary>
    /// Json SubCommand base
    /// </summary>
    /// <typeparam name="TSession">The type of the web socket session.</typeparam>
    /// <typeparam name="TRequest">The type of the request info.</typeparam>
    /// <typeparam name="TResponse">The type of the response info</typeparam>
    public abstract class GameCommandBase<TSession, TRequest, TResponse> : CommandBase<TSession, IGameRequestInfo>
        where TSession : IGameSession, IAppSession<TSession, IGameRequestInfo>, new()
    {
        //protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected ILog Logger { get; private set; }

        private Type m_CommandInfoType;

        protected object Args { get; set; }

        public GameCommandBase()
        {
            m_CommandInfoType = typeof(TRequest);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="requestInfo">The request info.</param>
        public override void ExecuteCommand(TSession session, IGameRequestInfo requestInfo)
        {
            Logger = session.AppServer.Logger;

            if (session.IsClosed)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"ExecuteCommand[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ffff")}]\t{session.SessionID}\t Session is closed!");
                return;
            }
            else
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"ExecuteCommand[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ffff")}]\t{session.SessionID}\tSessionID\t{Name}\tRid:{session.Rid}");
            }

            var cacheName = $"{Name}_{session.CurrentO}";
            if (requestInfo.IsRetry)
            { 
                //客户端请求失败 
              /*
                客户端请求失败 尝试重新请求
                服务端查看请求列表中是否有该请求
                有    则等待
                没有  则查看Response是否有
              */

                var appserver = (IGameServer) session.AppServer;


                if (appserver.ExistsReqeust(session, cacheName))
                {
                    // 已有请求 等待完成
                    var response = appserver.GetResponse<TResponse>(session, cacheName);
                    if (Equals(default(TResponse), response))
                    {
                        if (Logger.IsDebugEnabled)
                            Logger.Debug($"Retry fail,Session [{session.SessionID}],cacheName : {cacheName}");
                        return;
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

                        return;
                    }
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Retry no request,Session [{session.SessionID}],cacheName : {cacheName}");
                }
            }

            if (requestInfo.Body == null)
            {
                ExecuteGameCommand(session, default(TRequest));
            }
            else
            {
                var jsonCommandInfo = requestInfo.GetTypeBody<TRequest>();
                Args = jsonCommandInfo;
                ExecuteGameCommand(session, jsonCommandInfo);
            }

            //session.ProcessEx(requestInfo.Ex);
        }

        protected void SendResponse(TSession session, object response)
        {
            session.SendMessage(new ProtocolMessage
            {
                C = Name,
                D = response,
                O = session.CurrentO
            });
        }

        /// <summary>
        /// Executes the json command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="data">The command info.</param>
        protected abstract void ExecuteGameCommand(TSession session, TRequest data);
    }
}
