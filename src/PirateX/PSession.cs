using System;
using System.Net;
using Autofac;
using Newtonsoft.Json;
using PirateX.GException;
using PirateX.Protocol;
using PirateX.Redis.StackExchange.Redis.Ex;
using StackExchange.Redis;
using SuperSocket.SocketBase;

namespace PirateX
{
    public class PSession : PSession<PSession>
    {
    }

    public class PSession<TSession> : AppSession<TSession, IGameRequestInfo>, IGameSession where
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
        public ILifetimeScope Build
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


        public IProtocolPackage<IGameRequestInfo> ProtocolPackage { get; set;  }

        protected override void OnSessionStarted()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"SessionId:{this.SessionID}");
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
            if (!(e is AbstactGameException))
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

            if (e is AbstactGameException)
            {
                code = (e as AbstactGameException).CodeValue;
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

            if (!(e is AbstactGameException))
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

            var db = Build.Resolve<IDatabase>();
            if (db == null)
                return default(TResponse);

            var key = $"sys:response:{rid}:{c}";

            return db.Get<TResponse>(key);
        }

        public virtual void SetLastReponse(long rid, string c, object o)
        {
            if (rid <= 0 || string.IsNullOrEmpty(c) || o == null)
                return;

            var db = Build.Resolve<IDatabase>();
            if (db == null)
                return ;

            //这里来具体维护 缓存多少条 ~ 
            var key = $"sys:response:{rid}:{c}";
            var listkey = $"sys:response:{rid}";

                db.Set(key,o, new TimeSpan(0, 0, 1, 0));
            db.ListLeftPush(listkey, key);


            if (db.ListLength(listkey) >= 4)
            {
                var removekey = db.ListRightPop(listkey);
                db.KeyDelete(removekey.ToString());

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
                Logger.Info(string.Format("Response[{4}]\t#{0}#\t{1}\t{2}\t{3}", Rid, this.RemoteEndPoint, SessionID, JsonConvert.SerializeObject(message), result));
        }

        public virtual void ProcessedRequest(string name, object args, long pms, long sms, long ms, DateTime start, DateTime end, string o)
        {

        }
    }


}
