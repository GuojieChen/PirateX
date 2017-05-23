using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Newtonsoft.Json;
using NLog;
using PirateX.Core.Actor.ProtoSync;
using PirateX.Core.Broadcas;
using PirateX.Core.Container;
using PirateX.Core.Net;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Core.Session;
using PirateX.Core.Utils;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;
using ProtoBuf;
using StackExchange.Redis;

namespace PirateX.Core.Actor
{
    public interface IActorService
    {
        IActorNetService NetService { get; set; }

        void Setup();

        void Start();
        void Stop();

        void OnReceive(ActorContext context);
    }

    public class ActorService<TActorService> : IMessageSender, IActorService
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();

        public IActorNetService NetService { get; set; }

        private IDictionary<string, IAction> Actions = new Dictionary<string, IAction>(StringComparer.OrdinalIgnoreCase);

        public IServerContainer ServerContainer { get; set; }

        public IProtocolPackage ProtocolPackage { get; set; }

        protected ISessionManager OnlineManager { get; set; }

        protected virtual string DefaultResponseCovnert => "protobuf";


        public ActorService(IServerContainer serverContainer)
        {

            ServerContainer = serverContainer;
        }

        /// <summary>
        /// 加载各种配置
        /// </summary>
        public virtual void Setup()
        {
            var serverSetting = ServerContainer.GetServerSetting();

            var configtypes = serverSetting.GetType().GetInterfaces();

            var builder = new ContainerBuilder();

            //默认在线管理  
            builder.Register(c => new MemorySessionManager())
                .As<ISessionManager>()
                .SingleInstance();
            builder.Register(c => this).As<IMessageSender>().SingleInstance();
            foreach (var type in configtypes)
            {
                var attrs = type.GetCustomAttributes(typeof(ServerSettingRegisterAttribute), false);
                if (!attrs.Any())
                    continue;
                var attr = attrs[0] as ServerSettingRegisterAttribute;
                if (attr != null)
                    ((IServerSettingRegister)Activator.CreateInstance(attr.RegisterType))
                        .Register(builder, serverSetting);
            }


            ////默认的包解析器
            builder.Register(c => new ProtocolPackage())
                .InstancePerDependency()
                .As<IProtocolPackage>();

            //默认消息广播
            builder.Register(c => new DefaultMessageBroadcast()).SingleInstance();

            builder.Register(c => new ProtobufService()).As<IProtoService>().SingleInstance();

            //数据格式
            foreach (var responseConvert in typeof(IResponseConvert).Assembly.GetTypes().Where(item => typeof(IResponseConvert).IsAssignableFrom(item)))
            {
                if (responseConvert.IsInterface)
                    continue;

                var attrs = responseConvert.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                if (attrs.Any())
                {
                    var convertName = ((DisplayColumnAttribute)attrs[0]).DisplayColumn;
                    if (!string.IsNullOrEmpty(convertName))
                        builder.Register(c => Activator.CreateInstance(responseConvert))
                            .Keyed<IResponseConvert>(convertName.ToLower())
                            .SingleInstance();
                }
            }

            //注册内置命令
            RegisterActions(typeof(ActorService<TActorService>).Assembly.GetTypes());
            //注册外置命令
            RegisterActions(GetActions());

            builder.Register(c => Actions)
                .AsSelf()
                .SingleInstance();

            ServerContainer.InitContainers(builder);

            ServerContainer.ServerIoc.Resolve<IProtoService>().Init(ServerContainer.GetEntityAssemblyList());

            RedisDataBaseExtension.RedisSerilazer = ServerContainer.ServerIoc.Resolve<IRedisSerializer>();

            ProtocolPackage = ServerContainer.ServerIoc.Resolve<IProtocolPackage>();
            OnlineManager = ServerContainer.ServerIoc.Resolve<ISessionManager>();
        }

        public virtual void Start()
        {
        }

        private void RegisterActions(IEnumerable<Type> actions)
        {
            if (actions == null)
                return;

            foreach (var type in actions)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (!typeof(IAction).IsAssignableFrom(type))
                    continue;

                var action = ((IAction)Activator.CreateInstance(type));
                Actions.Add(string.IsNullOrEmpty(action.Name) ? type.Name : action.Name, action);
            }
        }

        /// <summary>
        /// 获取额外的接口列表
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Type> GetActions()
        {
            return typeof(TActorService).Assembly.GetTypes();
        }
        ///// <summary>
        ///// 多线程处理请求
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected override void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e)
        //{
        //    var msg = e.Socket.ReceiveMultipartMessage();
        //    if(Logger.IsDebugEnabled)
        //        Logger.Debug($"Pull,ActorService[{_id}],ThreadID:{Thread.CurrentThread.ManagedThreadId}");

        //    Task.Factory.StartNew(() => ProcessReceive(msg)).ContinueWith(t =>
        //    {
        //        //发生内部错误
        //        if (Logger.IsErrorEnabled)
        //            Logger.Error(t.Exception);

        //        //发生异常需要处理
        //        //t.Exception
        //    }, TaskContinuationOptions.OnlyOnFaulted);
        //}

        //处理接收

        public virtual void OnSessionClosed(PirateSession session)
        {

        }

        public void OnReceive(ActorContext context)
        {
            if (context.Action == 0)
            {
                return;
            }
            else if (context.Action == 2)//断线
            {
                if(Logger.IsDebugEnabled)
                    Logger.Debug("Session Logout ~");

                var session = OnlineManager.GetOnlineRole(context.SessionId);
                if (session != null)
                {
                    OnlineManager.Logout(session.Id);
                    OnSessionClosed(session);
                }

                return;
            }

            var token = GetToken(context.Request.Token);
            context.Token = token;

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"C2S Headers #{context.Token.Rid}# #{context.RemoteIp}# {context.Request.Headers}");
                Logger.Debug($"C2S Query #{context.Token.Rid}# #{context.RemoteIp}# {context.Request.QueryString}");
            }

            //授权检查
            if (!VerifyToken(ServerContainer.GetDistrictConfig(token.Did), token))
                throw new PirateXException("AuthError", "授权失败") { Code = StatusCode.Unauthorized };

            var format = context.Request.Headers["format"];
            if (!string.IsNullOrEmpty(format))
                context.ResponseCovnert = format;
            else
                context.ResponseCovnert = DefaultResponseCovnert;

            var lang = context.Request.Headers["lang"];
            if (!string.IsNullOrEmpty(lang))
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);

            //request timeout
            //if ((DateTime.UtcNow.GetTimestamp() - context.Request.Timestamp) > 1000 * 60 * 2)
            //{
            //    Logger.Warn($"C2S Timeout t #{context.SessionId}# #{context.RemoteIp}# {context.Request.Headers} ");
            //    return;
            //}

            if (context.Request.O <= context.LastNo)
            {
                Logger.Warn($"C2S Timeout o #{context.SessionId}# #{context.RemoteIp}# {context.Request.Headers} ");
                return;
            }

            //执行动作
            var actionname = context.Request.C;
            using (var action = GetActionInstanceByName(actionname))
            {
                if (action != null)
                {
                    try
                    {
                        if (Equals(actionname, "NewSeed"))
                        {

                        }
                        else
                        {
                            //session = OnlineManager.GetOnlineRole(token.Rid);
                            var container = ServerContainer.GetDistrictContainer(token.Did);
                            if (container == null)
                                throw new PirateXException("ContainerNull", "容器未定义") { Code = StatusCode.ContainerNull };
                            action.Reslover = container.BeginLifetimeScope();
                        }

                        action.ServerReslover = ServerContainer.ServerIoc;
                        action.Context = context;
                        action.Logger = Logger;
                        action.MessageSender = this;


                        action.Execute();

                        if (Equals(actionname, "NewSeed"))
                        {
                            //session 保存
                            OnlineManager.Login(ToSession(context,context.Token));
                        }
                    }
                    catch (Exception exception)
                    {
                        HandleException(context, exception);
                    }
                }
                else
                {
                    var headers = new NameValueCollection()
                    {
                        {"c", context.Request.C},
                        {"i", MessageType.Boradcast},
                        {"o", Convert.ToString(context.Request.O)},
                        {"code", Convert.ToString((int) StatusCode.NotFound)},
                        {"errorCode", "NotFound"},
                        {"errorMsg", $"Action {actionname} not found!"}
                    };
                    //返回类型 

                    SendMessage<string>(context, headers, null);
                }
            }

        }

        public virtual PirateSession ToSession(ActorContext context, IToken token)
        {
            var session = new PirateSession
            {
                SessionId = context.SessionId,
                Id = token.Rid,
                Did = token.Did,
                Token = context.Request.Token,
                Uid = token.Uid,
                StartUtcAt = DateTime.UtcNow,
                ResponseConvert = context.ResponseCovnert
            };

            return session;
        }

        /// <summary>
        /// 解析token信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual IToken GetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new PirateXException($"{StatusCode.BadRequest}", "invalidToken");
            var odatas = Convert.FromBase64String(token);

            return odatas.FromProtobuf<Token>();
        }
        /// <summary>
        /// 验证token 默认是不验证
        /// </summary>
        /// <param name="config"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual bool VerifyToken(IDistrictConfig config, IToken token)
        {
            var isign = $"{token.Did}{token.Rid}{token.Ts}{token.Uid}{config.SecretKey}".GetMD5();

            if (DateTime.UtcNow.GetTimestamp()/1000 - token.Ts >= 1000 * 60 * 60 *5)//5h
                return false;

            if (Equals(isign, token.Sign))
                return true;

            return false;
        }

        protected virtual void HandleException(ActorContext context, Exception e)
        {
            //#ERROR#
            var code = 400;

            string errorCode = string.Empty;
            string errorMsg;

            if (e is PirateXException)
            {
                var pe = (e as PirateXException);

                code = pe.Code;
                errorCode = pe.ErrorCode;
                errorMsg = pe.ErrorMsg;
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
                errorMsg = e.Message; //e.Message;

                if (Logger.IsErrorEnabled)
                    Logger.Error(e);
            }

            if (!(e is PirateXException))
            {
                //if (Logger.IsErrorEnabled)
                //    Logger.Error($"Exception [{ServerId}:{Rid}] - {e.Message} ", e);
            }


            var headers = new NameValueCollection
            {
                {"c", context.Request.C},
                {"i", MessageType.Boradcast},
                {"o", Convert.ToString(context.Request.O)},
                {"code", Convert.ToString(code)},
                {"errorCode", errorCode},
                {"guid",Guid.NewGuid().ToString("N")},
                {"errorMsg", HttpUtility.UrlEncode(errorMsg)}
            };
            //返回类型 

            SendMessage<string>(context, headers, null);
        }

        private IAction GetActionInstanceByName(string actionname)
        {
            IAction type;

            if (Actions.TryGetValue(actionname, out type))
            {
                var action = Activator.CreateInstance(type.GetType()) as IAction;

                return action;
            }
            return null;
        }

        public virtual void Stop()
        {

        }


        #region send message
        public void SendMessage<T>(ActorContext context, T t)
        {
            var headers = new NameValueCollection
            {
                {"c", context.Request.C},
                {"i", MessageType.Rep},
                {"o", Convert.ToString(context.Request.O)},
                {"code", Convert.ToString((int) StatusCode.Ok)}
            };

#if PERFORM
            foreach (string key in context.Request.Headers.AllKeys)
            {
                if (key.StartsWith("_"))
                    headers.Add(key, context.Request.Headers[key]);
            }
#endif


            SendMessage(context, headers, t);
        }

        /// <summary>
        /// 种子交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="t"></param>
        public void SendSeed<T>(ActorContext context, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, T t)
        {
            var headers = new NameValueCollection
            {
                {"c", context.Request.C},
                {"i", MessageType.Rep},
                {"o", Convert.ToString(context.Request.O)},
                {"code", Convert.ToString((int) StatusCode.Ok)}
            };
            //通知类型 


            headers["format"] = context.ResponseCovnert;

            var body = ServerContainer.ServerIoc.ResolveKeyed<IResponseConvert>(context.ResponseCovnert)
                .SerializeObject(t);

            if (Logger.IsDebugEnabled && body != null)
            {
                Logger.Debug($"S2C #{context.Token.Rid}# #{context.RemoteIp}# {string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a]))} {Encoding.UTF8.GetString(body)}");
            }

            NetService.Seed(context, headers, cryptobyte, clientkeys, serverkeys, body);
        }

        //public void PushMessage<T>(string sessionid, T t)
        //{
        //    var headers = new NameValueCollection
        //    {
        //        {"c", typeof(T).Name},
        //        { "i", MessageType.Boradcast},
        //        {"format","json"} // TODO 默认解析器
        //    };

        //    NetService.PushMessage(sessionid, headers, ServerContainer.ServerIoc.ResolveKeyed<IResponseConvert>("json").SerializeObject(t));
        //}


        public void PushMessage<T>(int rid, T t)
        {
            var headers = new NameValueCollection
            {
                {"c", typeof(T).Name},
                { "i", MessageType.Boradcast},
                {"format",DefaultResponseCovnert} // TODO 默认解析器
            };

            if (Logger.IsDebugEnabled && t != null)
            {
                Logger.Debug($"S2C #{rid}# {string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a]))} {JsonConvert.SerializeObject(t)}");
            }

            NetService.PushMessage(rid, headers, ServerContainer.ServerIoc.ResolveKeyed<IResponseConvert>(DefaultResponseCovnert).SerializeObject(t));
        }

        public void SendMessage<T>(ActorContext context, string name, T t)
        {
            var headers = new NameValueCollection
            {
                {"c", context.Request.C},
                {"i", MessageType.Boradcast},
                {"o", Convert.ToString(context.Request.O)},
                {"code", Convert.ToString((int) StatusCode.Ok)}
            };
            //通知类型 

            SendMessage(context, headers, t);
        }

        private void SendMessage<T>(ActorContext context, NameValueCollection header, T rep)
        {
            header["format"] = context.ResponseCovnert;

            var body = ServerContainer.ServerIoc.ResolveKeyed<IResponseConvert>(context.ResponseCovnert)
                .SerializeObject(rep);

            if (Logger.IsDebugEnabled && body != null)
            {
                Logger.Debug($"S2C #{context.Token.Rid}# #{context.RemoteIp}# {string.Join("&", header.AllKeys.Select(a => a + "=" + header[a]))} {Encoding.UTF8.GetString(body)}");
            }

            NetService.SendMessage(context, header, body);
        }

        private byte[] GetHeaderBytes(NameValueCollection headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a])));
        }

        #endregion
    }

    static class MessageType
    {
        public const string Rep = "1";

        public const string Boradcast = "2";
    }
}
