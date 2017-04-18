using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core.Broadcas;
using PirateX.Core.Container;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Net.Actor.Actions;
using PirateX.Net.Actor.ProtoSync;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;
using ProtoBuf;
using StackExchange.Redis;
using Topshelf.Logging;

namespace PirateX.Net.Actor
{
    public interface IActorService
    {
        void Start();
        void Stop();
    }

    public abstract class ActorService<TActorService,TOnlineRole> : ActorNetService, IMessageSender, IActorService
        where TOnlineRole : class, IOnlineRole, new()
    {
        private Guid _id = Guid.NewGuid();

        public static LogWriter Logger = HostLogger.Get(typeof(ActorService<TActorService,TOnlineRole>));

        protected IActorNetService NetService { get; set; }

        private IDictionary<string, IAction> Actions = new Dictionary<string, IAction>(StringComparer.OrdinalIgnoreCase);

        public static IServerContainer ServerContainer { get; set; }

        public IProtocolPackage ProtocolPackage { get; set; }

        protected IOnlineManager OnlineManager { get; set; }

        public ActorService(ActorConfig config, IServerContainer serverContainer)
            : base(config)
        {
            ServerContainer = serverContainer;
        }

        public override void Start()
        {
            base.SetUp();

            this.Setup();
            
            base.Start();
        }

        /// <summary>
        /// 加载各种配置
        /// </summary>
        public virtual void Setup()
        {
            var builder = new ContainerBuilder();
            //Redis连接池  管理全局信息
            builder.Register(c => ConnectionMultiplexer.Connect(ServerContainer.Settings.RedisHost))
                .As<ConnectionMultiplexer>()
                .SingleInstance();

            //在线管理
            builder.Register(c => new RedisOnlineManager<TOnlineRole>(c.Resolve<ConnectionMultiplexer>())
            {
                //Serializer = c.Resolve<IRedisSerializer>()
            }).As<IOnlineManager>()
              .SingleInstance();

            //builder.Register(c => new ProtoResponseConvert()).As<IResponseConvert>().SingleInstance();
            ////默认的包解析器
            builder.Register(c => new ProtocolPackage())
                .InstancePerDependency()
                .As<IProtocolPackage>();

            //默认消息广播
            builder.Register(c => new DefaultMessageBroadcast()).SingleInstance();

            builder.Register(c => new ProtobufService()).As<IProtoService>().SingleInstance();


            foreach (var responseConvert in typeof(IResponseConvert).Assembly.GetTypes().Where(item => typeof(IResponseConvert).IsAssignableFrom(item)))
            {
                if (responseConvert.IsInterface)
                    continue;

                var attrs = responseConvert.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                if (attrs.Any())
                {
                    var convertName = ((DisplayColumnAttribute)attrs[0]).DisplayColumn;
                    if(!string.IsNullOrEmpty(convertName))
                        builder.Register(c => Activator.CreateInstance(responseConvert))
                            .Keyed<IResponseConvert>(convertName.ToLower())
                            .SingleInstance();
                }
            }

            IocConfig(builder);
            ServerContainer.InitContainers(builder);

            ServerContainer.ServerIoc.Resolve<IProtoService>().Init(ServerContainer.ContainerSetting.EntityAssembly);

            RedisDataBaseExtension.RedisSerilazer = ServerContainer.ServerIoc.Resolve<IRedisSerializer>();

            ProtocolPackage = ServerContainer.ServerIoc.Resolve<IProtocolPackage>();
            OnlineManager = ServerContainer.ServerIoc.Resolve<IOnlineManager>();

            //注册内置命令
            RegisterActions(typeof(ActorService<TActorService,TOnlineRole>).Assembly.GetTypes());
            //注册外置命令
            RegisterActions(GetActions());
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
        /// <summary>
        /// 多线程处理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e)
        {
            var msg = e.Socket.ReceiveMultipartMessage();
            if(Logger.IsDebugEnabled)
                Logger.Debug($"Pull,ActorService[{_id}],ThreadID:{Thread.CurrentThread.ManagedThreadId}");

            Task.Factory.StartNew(() => ProcessReceive(msg)).ContinueWith(t =>
            {
                //发生内部错误
                if (Logger.IsErrorEnabled)
                    Logger.Error(t.Exception);

                //发生异常需要处理
                //t.Exception
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        //处理接收
        private void ProcessReceive(NetMQMessage msg)
        {
            var context = new ActorContext()
            {
                Version = msg[0].Buffer[0],//版本号
                                           //ActionName = msg[1].ConvertToString(),//动作
                SessionId = msg[2].ConvertToString(),//sessionid
                ClientKeys = msg[3].Buffer,//客户端密钥
                ServerKeys = msg[4].Buffer,//服务端密钥
                Request = new PirateXRequestInfo(
                        msg[5].Buffer, //信息头
                        msg[6].Buffer)//信息体
                        ,ResponseCovnert = "protobuf"
            };
            var format = context.Request.Headers["format"];
            if (!string.IsNullOrEmpty(format))
                context.ResponseCovnert = format;

            //执行动作
            var actionname = context.Request.C;
            using (var action = GetActionInstanceByName(actionname))
            {
                if (action != null)
                {
                    try
                    {
                        var token = GetToken(context.Request.Token);
                        context.Token = token;

                        //授权检查
                        if (!VerifyToken(ServerContainer.GetDistrictConfig(token.Did), token))
                            throw new PirateXException("AuthError", "授权失败") { Code = StatusCode.Unauthorized };

                        //context.Request.Token
                        //获取session信息  从缓存中去获取session信息  session没有的时候需要提示客户端重新连接

                        if (Equals(actionname, "NewSeed"))
                        {
                            var onlinerole2 = CreateOnlineRole(context, token);
                            onlinerole2.ClientKeys = context.ClientKeys;
                            onlinerole2.ServerKeys = context.ServerKeys;

                            ServerContainer.ServerIoc.Resolve<IOnlineManager>().Login(onlinerole2);
                        }
                        else
                        {
                            var onlinerole = OnlineManager.GetOnlineRole(token.Rid);
                            action.Reslover = ServerContainer.GetDistrictContainer(token.Did).BeginLifetimeScope();

                            if (onlinerole == null)
                            {
                                var onlinerole2 = CreateOnlineRole(context, token);
                                onlinerole2.ClientKeys = context.ClientKeys;
                                onlinerole2.ServerKeys = context.ServerKeys;

                                ServerContainer.ServerIoc.Resolve<IOnlineManager>().Login(onlinerole2);
                            }
                            else if (!Equals(onlinerole.SessionId, context.SessionId))
                            {
                                //单设备登陆控制
                                throw new PirateXException("ReLogin", "ReLogin") { Code = StatusCode.ReLogin };
                            }

                            //if (!context.ServerKeys.Any())
                            //{   //第一次请求

                            //}
                            //else
                            //{
                            //    //单设备登陆控制
                            //    if (!Equals(onlinerole.SessionId, context.SessionId))
                            //        throw new PirateXException("ReLogin", "ReLogin") { Code = StatusCode.ReLogin };
                            //}

                            action.OnlieRole = onlinerole;
                        }

                        action.ServerReslover = ServerContainer.ServerIoc;
                        action.Context = context;
                        action.Logger = Logger;
                        action.MessageSender = this;

                        action.Execute();
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
                        {"i", MessageType.Rep},
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

        protected abstract TOnlineRole CreateOnlineRole(ActorContext context, IToken token);

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

            using (var ms = new MemoryStream(odatas))
            {
                return Serializer.Deserialize<Token>(ms);
            }
        }
        /// <summary>
        /// 验证token 默认是不验证
        /// </summary>
        /// <param name="config"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual bool VerifyToken(IDistrictConfig config, IToken token)
        {
            //var signstr = $"{token.Did}{token.Rid}{token.Ts}{config.SecretKey}";

            //var isign = Utils.GetMd5(signstr);

            //return Equals(token.Sign, isign);

            return true;
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
                errorMsg = e.StackTrace; //e.Message;

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
                {"i", MessageType.Rep},
                {"o", Convert.ToString(context.Request.O)},
                {"code", Convert.ToString(code)},
                {"errorCode", errorCode},
                {"errorMsg", errorMsg}
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

        public virtual void IocConfig(ContainerBuilder builder)
        {

        }

        public override void Stop()
        {
            base.Stop();
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
            //返回类型 

            SendMessage(context, headers, t);
        }

        public void PushMessage<T>(IOnlineRole role, T t)
        {
            var headers = new NameValueCollection
            {
                {"c", typeof(T).Name},
                { "i", MessageType.Boradcast},
                {"format",role.ResponseConvert}
            };

            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { 1 });//版本号
            repMsg.Append("action");//动作
            repMsg.Append(role.SessionId);//sessionid
            repMsg.Append(role.ClientKeys);//客户端密钥
            repMsg.Append(role.ServerKeys);//服务端密钥
            repMsg.Append(GetHeaderBytes(headers));//信息头
            if (!Equals(t, default(T)))
                repMsg.Append(ServerContainer.ServerIoc.ResolveKeyed<IResponseConvert>(role.ResponseConvert).SerializeObject(t));//信息体

            base.EnqueueMessage(repMsg);
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

            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { context.Version });//版本号
            repMsg.Append("action");//动作
            repMsg.Append(context.SessionId);//sessionid
            repMsg.Append(context.ClientKeys);//客户端密钥
            repMsg.Append(context.ServerKeys);//服务端密钥
            repMsg.Append(GetHeaderBytes(header));//信息头
            if (!Equals(rep, default(T)))
                repMsg.Append(ServerContainer.ServerIoc.ResolveKeyed<IResponseConvert>(context.ResponseCovnert).SerializeObject(rep));//信息体
            else
                repMsg.AppendEmptyFrame();

            base.EnqueueMessage(repMsg);
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
