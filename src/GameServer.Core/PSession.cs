using System;
using System.Net;
using System.Text;
using Autofac;
using GameServer.Core.GException;
using GameServer.Core.Protocol;
using ServiceStack;
using ServiceStack.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using IGameSession = GameServer.Core.IGameSession;

namespace GameServer.Core
{
    public class PSession : PSession<PSession, Enum>, IAppSession<PSession, IGameRequestInfo>
    {
    }

    public class PSession<TSession, TErrorCode> : AppSession<TSession, IGameRequestInfo>, IGameSession where
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
        private ILifetimeScope Build
        {
            get
            {
                if (_container == null)
                    return _fContainer;

                return _container;
            }
            set
            {
                if (_container == null)
                    _container = value.BeginLifetimeScope();
                _fContainer = value;
            }
        }
        #endregion
        
        public IProtocolPackage<IGameRequestInfo> ProtocolPackage { get; set;  }

        protected override void OnSessionStarted()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"SessionId:{this.SessionID}");
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            if (Logger.IsDebugEnabled)
                Logger.Debug($"Session {reason}:{this.SessionID}");

            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }

        protected override void HandleException(System.Exception e)
        {
            if (!(e is AbstactGameException<TErrorCode>))
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
            object code = null;
            string msg = null;

            if (e is AbstactGameException<TErrorCode>)
            {
                code = (e as AbstactGameException<TErrorCode>).CodeValue;
                msg = e.ToString();
            }
            else if (e is WebException)
            {
                //TODO 
                //code = (short)ServerCode.RemoteError;
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

            if (!(e is AbstactGameException<TErrorCode>))
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error($"Exception [{ServerId}:{Rid}] - {e.Message} ", e);
            }

            try
            {
                this.SendMessage(new
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
            //SendError(requestInfo.Key, new AbstactGameException(ServerCode.NotFound, requestInfo.Key));
            if(Logger.IsErrorEnabled)
                Logger.Error($"Unknow request\t:\t{requestInfo.Key}");
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        #region 请求结果的缓存

        public virtual TResponse GetLastResponse<TResponse>(long rid, string c)
        {
            if (rid <= 0)
                return default(TResponse);

            var rm = Build.Resolve<IRedisClientsManager>();
            if (rm == null)
                return default(TResponse);

            var key = $"sys:response:{rid}:{c}";

            using (var redis = rm.GetClient())
                return redis.Get<TResponse>(key);
        }

        public virtual void SetLastReponse(long rid, string c, object o)
        {
            if (rid <= 0 || string.IsNullOrEmpty(c) || o == null)
                return;

            var rm = Build.Resolve<IRedisClientsManager>();
            if (rm == null)
                return;

            //这里来具体维护 缓存多少条 ~ 
            var key = $"sys:response:{rid}:{c}";
            var listkey = $"sys:response:{rid}";

            using (var redis = rm.GetClient())
            {
                redis.Set(key, o, new TimeSpan(0, 0, 1, 0));//30秒缓存
                redis.EnqueueItemOnList(listkey, key);

                if (redis.GetListCount(listkey) >= 4)
                {
                    var removekey = redis.DequeueItemFromList(listkey);
                    redis.Remove(removekey);
                }
            }
        }
        #endregion

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam cmdName="TMessage"></typeparam>
        /// <param cmdName="message"></param>
        public void SendMessage<TMessage>(TMessage message)
        {
            byte[] data = ProtocolPackage.SerializeObject(message);

            var result = TrySend(data, 0, data.Length);

            if (Logger.IsInfoEnabled)
                Logger.Info(string.Format("Response[{4}]\t#{0}#\t{1}\t{2}\t{3}", Rid, this.RemoteEndPoint, SessionID, message?.ToJsv(), result));
        }

        public virtual void ProcessedRequest(string name, object args, long pms, long sms, long ms, DateTime start, DateTime end, string o)
        {

        }
    }


}
