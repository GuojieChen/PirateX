using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Autofac;
using Newtonsoft.Json;
using NLog;
using PirateX.Protocol;
using PirateX.Protocol.ResponseConvert;

namespace PirateX.Core
{
    public interface IActorService
    {
        IActorNetService ActorNetService { get; set; }
        //void Setup();

        void Start();

        void Stop();

        byte[] OnReceive(ActorContext context);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TActorService"></typeparam>
    public class ActorService<TActorService> : IActorService,IMessageSender
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();

        public IActorNetService ActorNetService { get; set; }

        private IDictionary<string, IAction> Actions = new Dictionary<string, IAction>(StringComparer.OrdinalIgnoreCase);

        public IDistrictContainer DistrictContainer { get; set; }

        public IProtocolPackage ProtocolPackage { get; set; }

        protected virtual string DefaultResponseCovnert => "protobuf";

        protected int OnlineCount { get; set; }

        public ActorService(IDistrictContainer districtContainer)
        {
            DistrictContainer = districtContainer;

        }

        /// <summary>
        /// 
        /// </summary>
        private void Setup()
        {
            var builder = new ContainerBuilder();
            #region 通信相关组件

            builder.Register(c => this).As<IMessageSender>().SingleInstance();
            ////注册内置命令
            //RegisterActions(typeof(ActorService<TActorService>).Assembly.GetTypes());
            ////注册外置命令
            //RegisterActions(GetActions());

            //注册命令
            RegisterActions(DistrictContainer.GetApiAssemblyList());

            builder.Register(c => Actions)
                .As<IDictionary<string, IAction>>()
                .SingleInstance();

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

            #endregion
            // 游戏容器
            DistrictContainer.InitContainers(builder);
            #region 通信框架初始化

            if (DistrictContainer.ServerIoc.IsRegistered<IProtoService>())
            {
                var list = new List<Assembly>();
                list.AddRange(DistrictContainer.GetServiceAssemblyList());
                list.AddRange(DistrictContainer.GetEntityAssemblyList());
                list.AddRange(DistrictContainer.GetApiAssemblyList());
                DistrictContainer.ServerIoc.Resolve<IProtoService>()
                    .Init(list);
            }

            RedisDataBaseExtension.RedisSerilazer = DistrictContainer.ServerIoc.Resolve<IRedisSerializer>();
            if (Logger.IsTraceEnabled)
                Logger.Trace($"Set RedisDataBaseExtension.RedisSerilazer = {RedisDataBaseExtension.RedisSerilazer.GetType().FullName}");

            ProtocolPackage = DistrictContainer.ServerIoc.Resolve<IProtocolPackage>();
            if (Logger.IsTraceEnabled)
                Logger.Trace($"Set ProtocolPackage = {ProtocolPackage.GetType().FullName}");
            #endregion

            if (Logger.IsTraceEnabled)
                Logger.Trace(@"
.______    __  .______          ___   .___________. __________   ___ 
|   _  \  |  | |   _  \        /   \  |           ||   ____\  \ /  / 
|  |_)  | |  | |  |_)  |      /  ^  \ `---|  |----`|  |__   \     /  
|   ___/  |  | |      /      /  /_\  \    |  |     |   __|   >   <   
|  |      |  | |  |\  \----./  _____  \   |  |     |  |____ /  .  \  
| _|      |__| | _| `._____/__/     \__\  |__|     |_______/__/ \__\ 
");

        }

        public virtual void Start()
        {
            Setup();
        }

        private void RegisterActions(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                RegisterActions(assembly.GetTypes());
            }
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
            OnlineCount--;
        }

        public virtual void Ping(string servername,int onlinecount)
        {

        }

        public byte[] OnReceive(ActorContext context)
        {
            if (context.Action == 0)
            {
                Logger.Debug("ping");

                var onlinecount = 0;
                if (context.ServerItmes.ContainsKey("OnlineCount"))
                    onlinecount = Convert.ToInt32(context.ServerItmes["OnlineCount"]);

                Ping(context.ServerName, onlinecount);
                return null; 
            }
            else if (context.Action == 2)//断线
            {
                if(Logger.IsDebugEnabled)
                    Logger.Debug($"Session[{context.SessionId}] Logout ~");

                var session = DistrictContainer.OnlineManager.GetSession(context.SessionId);
                if (session != null)
                {
                    DistrictContainer.OnlineManager.Logout(session.Id);
                    OnSessionClosed(session);
                }

                return null ;
            }

            var token = GetToken(context.Request.Token);
            context.Token = token;

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"C2S Headers #{context.Token.Rid}# #{context.RemoteIp}# {context.Request.Headers}");
                Logger.Debug($"C2S Query #{context.Token.Rid}# #{context.RemoteIp}# {context.Request.QueryString}");
            }

            //授权检查
            if (!VerifyToken(DistrictContainer.GetDistrictConfig(token.Did), token))
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
                return null ;
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
                            OnSessionConnected(ToSession(context, context.Token));
                        }
                        else
                        {
                            //session = OnlineManager.GetSession(token.Rid);
                            var container = DistrictContainer.GetDistrictContainer(token.Did);
                            action.Resolver = container ?? throw new PirateXException("ContainerNull", "容器未定义") { Code = StatusCode.ContainerNull }; //.BeginLifetimeScope();
                        }

                        action.ServerReslover = DistrictContainer.ServerIoc;
                        action.Context = context;
                        action.Logger = Logger;
                        action.MessageSender = this;

                        action.Execute();

                        if (Equals(actionname, "NewSeed"))
                        {
                            //session 保存
                            DistrictContainer.OnlineManager.Login(ToSession(context,context.Token));
                        }

                        var result = action.ResponseData;
                        if (result == null)
                            return this.SendMessage(context,string.Empty);
                        return result;
                    }
                    catch (Exception exception)
                    {
                        return HandleException(context, exception);
                    }
                }
            }

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

            return SendMessage<string>(context, headers, null);
        }
         
        public virtual void OnSessionConnected(PirateSession session)
        {
            OnlineCount++;
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
                StartTimestamp = DateTime.UtcNow.GetTimestampAsSecond(),
                ResponseConvert = context.ResponseCovnert,
                ServerName = context.ServerName,
                FrontendID = context.FrontendID,
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

            if (TimeUtil.GetTimestamp(DateTime.UtcNow)/1000 - token.Ts >= 1000 * 60 * 60 *5)//5h
                return false;

            if (Equals(isign, token.Sign))
                return true;

            return false;
        }

        protected virtual byte[] HandleException(ActorContext context, Exception e)
        {
            //#ERROR#
            short code = StatusCode.Exception;

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
                code = StatusCode.ServerError;
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

            return SendMessage<string>(context, headers, null);
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
        /// <summary>
        /// 
        /// </summary>
        public virtual void Stop()
        {

        }

        #region send message
        /// <summary>
        /// 种子交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="t"></param>
        public byte[] SendSeed<T>(ActorContext context, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, T t)
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

            if (Equals(context.ResponseCovnert, "protobuf"))
                headers["responsetype"] = typeof(T).Name;

            var body = DistrictContainer.ServerIoc.ResolveKeyed<IResponseConvert>(context.ResponseCovnert)
                .SerializeObject(t);

            if (Logger.IsDebugEnabled && body != null)
            {
                Logger.Debug($"S2C #{context.Token.Rid}# #{context.RemoteIp}# {string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a]))} {Encoding.UTF8.GetString(body)}");
            }

            return ActorNetService.Seed(context, headers, cryptobyte, clientkeys, serverkeys, body);
        }

        public void PushMessage<T>(PirateSession session, T t)
        {
            var headers = new NameValueCollection
            {
                {"c", typeof(T).Name},
                { "i", MessageType.Boradcast},
                {"format",DefaultResponseCovnert} // TODO 默认解析器
            };

            if (Equals(DefaultResponseCovnert, "protobuf"))
                headers["responsetype"] = typeof(T).Name;


            if (Logger.IsDebugEnabled && t != null)
            {
                Logger.Debug($"S2C PUSH #{session.Id}# {string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a]))} {JsonConvert.SerializeObject(t)}");
            }

            ActorNetService.PushMessage(session.FrontendID, headers, DistrictContainer.ServerIoc.ResolveKeyed<IResponseConvert>(DefaultResponseCovnert).SerializeObject(t));
        }

        public void PushMessage<T>(int rid, T t)
        {
            var session = DistrictContainer.OnlineManager.GetSession(rid);
            if (session == null)
                return;

            PushMessage(session,t);
        }

        public void PushMessageToDistrict<T>(int did, T t)
        {
            //DistrictContainer.OnlineManager.GetSession()
        }

        public byte[] SendMessage<T>(ActorContext context, T t)
        {
            var headers = new NameValueCollection
            {
                {"c", context.Request.C},
                {"i", MessageType.Rep},
                {"o", Convert.ToString(context.Request.O)},
                {"code", Convert.ToString((int) StatusCode.Ok)}
            };
            //通知类型 
            if (Equals(DefaultResponseCovnert, "protobuf"))
                headers["responsetype"] = typeof(T).Name;

            return SendMessage(context, headers, t);
        }

        public byte[] SendMessage(ActorContext context,string msg)
        {
            var headers = new NameValueCollection
            {
                {"c", context.Request.C},
                {"i", MessageType.Rep},
                {"o", Convert.ToString(context.Request.O)},
                {"code", Convert.ToString((int) StatusCode.Ok)},
                { "format" , "json"}
            };
            //通知类型 

            return ActorNetService.SendMessage(context, headers, Encoding.UTF8.GetBytes(msg));
        }

        private byte[] SendMessage<T>(ActorContext context, NameValueCollection header, T rep)
        {
            header["format"] = context.ResponseCovnert;

            var body = DistrictContainer.ServerIoc.ResolveKeyed<IResponseConvert>(context.ResponseCovnert)
                .SerializeObject(rep);


            if (Equals(context.ResponseCovnert, "protobuf"))
                header["responsetype"] = typeof(T).Name;

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"S2C SEND #{context.Token.Rid}# #{context.RemoteIp}# {string.Join("&", header.AllKeys.Select(a => a + "=" + header[a]))} {(body==null?"":Encoding.UTF8.GetString(body))}");
            }

            return ActorNetService.SendMessage(context, header, body);
        }

        private byte[] GetHeaderBytes(NameValueCollection headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a])));
        }

        #endregion
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    static class MessageType
    {
        /// <summary>
        /// 请求返回 消息
        /// </summary>
        public const string Rep = "1";
        /// <summary>
        /// 推送 消息
        /// </summary>
        public const string Boradcast = "2";
    }
}
