using System;
using System.Collections.Specialized;
using Autofac.Core.Activators;
using PirateX.Protocol.Package;
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
    public abstract class GameCommandBase<TSession, TRequest> : CommandBase<TSession, IPirateXRequestInfo>
        where TSession : IPirateXSession, IAppSession<TSession, IPirateXRequestInfo>, new()
    {
        protected ILog Logger { get; private set; }

        public IPirateXRequestInfoBase Request;

        public IPirateXResponseInfo Response;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="requestInfo">The request info.</param>
        public override void ExecuteCommand(TSession session, IPirateXRequestInfo requestInfo)
        {
            Request = requestInfo;
            Response = new PirateXResponseInfo();
            Response.Headers.Add("c", requestInfo.Key);
            Response.Headers.Add("i","1");//返回类型 
            Response.Headers.Add("o",Convert.ToString(requestInfo.O));
            Response.Headers.Add("code",Convert.ToString((int)StatusCode.Ok));

            Logger = session.AppServer.Logger;

            if (session.IsClosed)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"ExecuteCommand[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ffff")}]\t{session.SessionID}\t Session is closed!");
                return;
            }

            var cacheName = $"{Name}_{session.CurrentO}";
            if (requestInfo.R)
            {
                if (Retry(session, cacheName))
                    return;
            }

            if (requestInfo.QueryString == null)
            {
                ExecuteGameCommand(session, default(TRequest));
            }
            else
            {
                var jsonCommandInfo = ConvertFromQueryString(requestInfo.QueryString);
                ExecuteGameCommand(session, jsonCommandInfo);
            }
        }

        protected virtual TRequest ConvertFromQueryString(NameValueCollection parameters)
        {
            var instance = Activator.CreateInstance<TRequest>();
            var type = typeof (TRequest);
            foreach (string name in parameters)
            {
                var value = parameters[name];

                //数据校验
                var p = type.GetProperty(name);
                p?.SetValue(instance, value);
            }

            return instance; 
        }

        protected virtual bool Retry(TSession session, string cacheName)
        {
            return false;
        }

        /// <summary>
        /// Executes the json command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="data">The command info.</param>
        protected abstract void ExecuteGameCommand(TSession session, TRequest data);
    }
}
