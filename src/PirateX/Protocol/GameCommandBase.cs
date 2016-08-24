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
    public abstract class GameCommandBase<TSession, TRequest> : CommandBase<TSession, IGameRequestInfo>
        where TSession : IGameSession, IAppSession<TSession, IGameRequestInfo>, new()
    {
        protected ILog Logger { get; private set; }

        protected object Args { get; set; }

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
                if (Retry(session, cacheName))
                    return;
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
        }

        protected virtual bool Retry(TSession session, string cacheName)
        {
            return false;
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
