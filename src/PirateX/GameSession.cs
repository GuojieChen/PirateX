using System;
using System.Linq;
using System.Net;
using Autofac;
using Newtonsoft.Json;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.GException;
using PirateX.GException.V1;
using PirateX.Protocol;
using StackExchange.Redis;
using SuperSocket.SocketBase;

namespace PirateX
{
    public class GameSession : GameSession<GameSession>
    {
    }

    public class GameSession<TSession> : AppSession<TSession, IGameRequestInfo>, IGameSession where
        TSession : AppSession<TSession, IGameRequestInfo>, new()

    {
        public bool IsLogin { get; set; }
        public bool IsClosed { get; set; }
        public long Rid { get; set; }
        public DateTime LastResponseTime { get; set; }
        public int CurrentO { get; set; }
        public int MyLastO { get; set; }

        public int ServerId { get; set; }

        #region IOC
        private ILifetimeScope _container;
        private ILifetimeScope _fContainer;
        //GameServer  IOC
        public ILifetimeScope Reslover
        {
            get
            {
                if (_container == null)
                    return _fContainer;

                return _container;
            }
            set
            {
                if (value == null)
                    return;

                if (_container == null)
                    _container = value.BeginLifetimeScope();
                _fContainer = value;
            }
        }
        #endregion
        public IProtocolPackage ProtocolPackage { get; set; }

        protected override void OnSessionStarted()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"OnSessionStarted - SessionId:{this.SessionID}");
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);

            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }

        protected override void HandleException(System.Exception e)
        {
            if (!(e is GameException))
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error(e.Message, e);
            }
            if (string.IsNullOrEmpty(CurrentCommand))
                SendError("Undefined", e);
            else
                SendError(CurrentCommand, e);
        }

        private void SendError(string cmdName, System.Exception e)
        {
            //#ERROR#
            object code = 400;
            string msg = null;

            if (e is GameException)
            {
                code = (e as GameException).Code;
                msg = e.ToString();
            }
            else if (e is WebException)
            {
                code = (short)ServerCode.RemoteError;
                msg = e.Message;
            }
            //else if (e is JsonReaderException)
            //{
            //    //TODO 
            //    //code = (short) ServerCode.BadRequest;
            //    msg = e.Message;
            //}
            else
            {
                msg = "ServerError"; //e.Message;
            }

            if (!(e is GameException))
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error($"Exception [{ServerId}:{Rid}] - {e.Message} ", e);
            }

            try
            {
                this.SendMessage(new ProtocolMessage()
                {
                    C = cmdName,
                    Code = code,
                    Msg = msg,
                    O = CurrentO
                });
            }
            catch (System.Exception exception)
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error(exception.Message, exception);
            }
        }

        protected override void HandleUnknownRequest(IGameRequestInfo requestInfo)
        {
            SendError(requestInfo.Key, new GameException(ServerCode.NotFound, requestInfo.Key));
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam cmdName="TMessage"></typeparam>
        /// <param cmdName="message"></param>
        /// <param name="message"></param>
        public void SendMessage(ProtocolMessage message)
        {
            byte[] data = ProtocolPackage.SerializeObject(message);

            var result = TrySend(data, 0, data.Length);

            if (Logger.IsInfoEnabled)
                Logger.Info(string.Format("Response[{4}]\t#{0}#\t{1}\t{2}\t{3}", Rid, this.RemoteEndPoint, SessionID, JsonConvert.SerializeObject(message), result));
        }

        public virtual void ProcessedRequest(string name, object args, long pms, long sms, long ms, DateTime start, DateTime end, string o)
        {

        }
    }


}
