﻿using System;
using System.Net;
using System.Text;
using GameServer.Core.GException;
using GameServer.Core.Json;
using GameServer.Core.Package;
using GameServer.Core.Protocol.PokemonX;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using IGameSession = GameServer.Core.IGameSession;

namespace GameServer.Core
{
    public class PSession : PSession<PSession,Enum>, IAppSession<PSession, IRequestInfo>
    {
        public PSession(IPackageProcessor packageProcessor) 
            : base(packageProcessor)
        {

        }

        public PSession()
            : base()
        {

        }

        public override void Initialize(IAppServer<PSession, IRequestInfo> server, ISocketSession socketSession)
        {
            base.Initialize(server,socketSession);
        }
    }

    public class PSession<TSession,TErrorCode> : AppSession<TSession, IRequestInfo>, IGameSession where 
        TSession : AppSession<TSession, IRequestInfo>, new()
        
    {
        public bool IsClosed { get; set; }
        public long Rid { get; set; }
        public DateTime LastResponseTime { get; set; }
        public int CurrentO { get; set; }
        public int MyLastO { get; set; }

        public int ServerId { get; set; }

        public IPackageProcessor PackageProcessor { get; set; }

        public PSession(IPackageProcessor packageProcessor)
        {
            PackageProcessor = packageProcessor;
        }

        public PSession()
        {
            PackageProcessor = new DefaultPackageProcessor();
        }

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
        }

        protected override void HandleException(System.Exception e)
        {
            if (!(e is AbstactGameException<TErrorCode> ))
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
            else if (e is JsonReaderException)
            {
                //TODO 
                //code = (short) ServerCode.BadRequest;
                msg = e.Message;
            }
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

        protected override void HandleUnknownRequest(IRequestInfo requestInfo)
        {
            //SendError(requestInfo.Key, new AbstactGameException(ServerCode.NotFound, requestInfo.Key));
        }

        #region Send Json Data

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam cmdName="TMessage"></typeparam>
        /// <param cmdName="message"></param>
        public void SendMessage<TMessage>(TMessage message)
        {
            byte[] data = null;

            data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, JsonSettings));

            SendData(data);
        }
        
        public virtual void ProcessedRequest(string name,object args, long pms, long sms, long ms,DateTime start,DateTime end,string o)
        {

        }

        public virtual object GetLastResponse(long rid, string c)
        {
            return null;
        }

        public virtual void SetLastReponse(long rid, string c, object o)
        {
        }

        public virtual bool SetLastRequest(long rid, string c,int mill)
        {
            return false; 
        }

        public virtual void SendData(byte[] sendData)
        {
            var data = PackageProcessor.Pack(sendData);
            
            var result = TrySend(data, 0, data.Length);

            if (Logger.IsDebugEnabled)
                Logger.Debug(string.Format("Response[{4}]\t#{0}#\t{1}\t{2}\t{3}", Rid, this.RemoteEndPoint, SessionID, Encoding.UTF8.GetString(sendData), result));
        }

        /// <summary>
        /// JSON 序列化配置
        /// </summary>
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                var timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                };

                settings.Converters.Add(timeConverter);
                settings.Converters.Add(new DoubleConverter());

                return settings;
            }
        }

        #endregion
    }

    
}
