using System;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Logging;

namespace GameServer.Core.PkProtocol
{
    /// <summary>
    /// Json SubCommand base
    /// </summary>
    /// <typeparam name="TSession">The type of the web socket session.</typeparam>
    /// <typeparam name="TJsonCommandInfo">The type of the json command info.</typeparam>
    public abstract class JsonSubCommandBase<TSession, TJsonCommandInfo> : CommandBase<TSession, ISocketRequestInfo>
        where TSession : IGameSession, IAppSession<TSession, ISocketRequestInfo>, new()
    {
        //protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected ILog Logger { get; private set; } 

        private Type m_CommandInfoType;

        protected object Args { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSubCommandBase&lt;TWebSocketSession, TJsonCommandInfo&gt;"/> class.
        /// </summary>
        public JsonSubCommandBase()
        {
            m_CommandInfoType = typeof(TJsonCommandInfo);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="requestInfo">The request info.</param>
        public override void ExecuteCommand(TSession session, ISocketRequestInfo requestInfo)
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

            //TODO 如果上一个请求方法相同 但还没有执行完，本次请求返回等待异常

            //请求频率监控
            //获取相同请求最后一次监控
            //if (RequstInterval.HasValue)
            //{
            //    //设置失败 请求过高
            //    if (!session.SetLastRequest(session.Rid, Name, RequstInterval.Value))
            //        throw new PException(ServerCode.RepeatedRequest);
            //}

            //var defaultCulture = session.AppServer.Config.Options.GetValue("defaultCulture");
            //if (!string.IsNullOrEmpty(defaultCulture))
            //    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(defaultCulture); 
            
            if (requestInfo.R)
            { //客户端请求失败 
                var cacheName = Convert.ToString(session.CurrentO);
                var r = session.GetLastResponse(session.Rid, cacheName);

                if (r != null)
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Retry success,Session [{session.SessionID}]");

                    session.SendMessage(new
                    {
                        C = Name,
                        D = r,
                        O = session.CurrentO
                    });

                    return;
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug($"Retry fail,Session [{session.SessionID}]");
                }
            }

            if (requestInfo.Body == null)
            {
                ExecuteJsonCommand(session, default(TJsonCommandInfo));
            }
            else
            {
                var jsonCommandInfo = (TJsonCommandInfo)requestInfo.Body.ToObject(m_CommandInfoType);
                Args = jsonCommandInfo;
                ExecuteJsonCommand(session, jsonCommandInfo);
            }

            //session.ProcessEx(requestInfo.Ex);
        }

        protected void SendResponse(TSession session, object response)
        {
            session.SendMessage(new
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
        protected abstract void ExecuteJsonCommand(TSession session, TJsonCommandInfo data);
    }
}
