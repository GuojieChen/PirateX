using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Autofac;
using Newtonsoft.Json;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using StackExchange.Redis;
using SuperSocket.SocketBase;

namespace PirateX
{
    public class PirateXSession : PirateXSession<PirateXSession>
    {
    }

    public class PirateXSession<TSession> : AppSession<TSession, IPirateXRequestInfo>, IPirateXSession where
        TSession : AppSession<TSession, IPirateXRequestInfo>, new()

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
        //PirateXServer  IOC
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
                Logger.Debug($"OnSessionStarted - SessionId:{SessionID}");
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

        protected override void HandleException(Exception e)
        {
            if (!(e is PirateXException))
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error(e.Message, e);
            }
            if (string.IsNullOrEmpty(CurrentCommand))
                SendError("Undefined", e);
            else
                SendError(CurrentCommand, e);
        }

        private void SendError(string cmdName, Exception e)
        {
            //#ERROR#
            var code = 400;

            string errorCode;
            string errorMsg;

            if (e is PirateXException)
            {
                var pe = (e as PirateXException);

                errorCode = pe.ErrorCode;
                errorMsg = pe.ErrorMessage;
            }
            else if (e is WebException)
            {
                code = (short)StatusCode.RemoteError;
                errorCode = StatusCode.RemoteError.ToString();
                errorMsg = e.Message;
            }
            else
            {
                errorCode = "ServerError";
                errorMsg = "ServerError"; //e.Message;
            }

            if (!(e is PirateXException))
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error($"Exception [{ServerId}:{Rid}] - {e.Message} ", e);
            }

            var response = new PirateXResponseInfo();
            response.Headers.Add("c", cmdName);
            response.Headers.Add("code", $"{Convert.ToInt32(code)}");
            response.Headers.Add("errorCode", errorCode);
            response.Headers.Add("errorMsg", errorMsg);
            response.Headers.Add("o", Convert.ToString(CurrentO));

            SendMessage(response);
        }

        protected override void HandleUnknownRequest(IPirateXRequestInfo requestInfo)
        {
            SendError(requestInfo.Key, new PirateXException(StatusCode.NotFound, requestInfo.Key));
        }
        
        private void SendMessage(PirateXResponseInfo response)
        {
            SendMessage<object>(response,null);
        }

        public void SendMessage<T>(IPirateXResponseInfo responseInfo, T data)
        {
            var responsePack = new PirateXResponsePackage()
            {
                HeaderBytes = responseInfo.GetHeaderBytes(),
                ContentBytes = ProtocolPackage.ResponseConvert.SerializeObject(data)
            };

            var senddatas = ProtocolPackage.PackResponsePackageToBytes(responsePack);

            Send(senddatas, 0, senddatas.Length);

            Logger.Debug("SEND MESSAGE!!!!");
        }

        /// <summary>
        /// 下发消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void SendMssage<T>(string name,T data)
        {
            var response = new PirateXResponseInfo();
            response.Headers.Add("c",name);
            response.Headers.Add("i","2");
            response.Headers.Add("code",$"{StatusCode.Ok}");

            SendMessage(response,data);
        }



        public virtual void ProcessedRequest(string name, NameValueCollection args, long pms, long sms, long ms, DateTime start, DateTime end, string o)
        {

        }
    }


}
