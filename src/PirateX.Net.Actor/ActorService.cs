using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core.Broadcas;
using PirateX.Core.Container;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Net.Actor.Actions;
using PirateX.Protocol;
using PirateX.Protocol.Package;
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

    public class ActorService<TOnlineRole>:IMessageSender, IActorService
        where TOnlineRole : class, IOnlineRole, new()
    {
        public static LogWriter Logger = HostLogger.Get(typeof (ActorService<TOnlineRole>));

        private PullSocket PullSocket { get; set; }
        private PushSocket PushSocket { get; set; }

        private NetMQPoller Poller { get; set; }
        private readonly NetMQQueue<NetMQMessage> MessageQueue = new NetMQQueue<NetMQMessage>();

        private IDictionary<string, IAction> Actions = new Dictionary<string, IAction>(StringComparer.OrdinalIgnoreCase);

        public static IServerContainer ServerContainer { get; set; }

        public IProtocolPackage ProtocolPackage { get; set; }

        private ActorConfig Config { get; }

        protected IOnlineManager OnlineManager { get; set; } 

        public ActorService(ActorConfig config, IServerContainer serverContainer)
        {
            ServerContainer = serverContainer;
            Config = config;
        }

        public void Start()
        {
            Setup();
            Poller.RunAsync();
        }

        /// <summary>
        /// 加载各种配置
        /// </summary>
        public virtual void Setup()
        {
            PullSocket = new PullSocket(Config.PullConnectHost);
            PullSocket.ReceiveReady += ProcessTaskPullSocket;

            PushSocket = new PushSocket(Config.PushConnectHost);
            Poller = new NetMQPoller() { PullSocket , PushSocket, MessageQueue };

            //var builder = new ContainerBuilder();
            MessageQueue.ReceiveReady += ProcessSend;
            
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

            builder.Register(c => new ProtoResponseConvert()).As<IResponseConvert>().SingleInstance();
            //默认的包解析器
            builder.Register(c => new ProtocolPackage(c.Resolve<IResponseConvert>()))
                .InstancePerDependency()
                .As<IProtocolPackage>();
            
            //默认消息广播
            builder.Register(c => new DefaultMessageBroadcast()).SingleInstance();

            //builder.Register(c => new ProtobufService()).As<IProtoService>().SingleInstance();

            IocConfig(builder);
            ServerContainer.InitContainers(builder);

            RedisDataBaseExtension.RedisSerilazer = ServerContainer.ServerIoc.Resolve<IRedisSerializer>();

            ProtocolPackage = ServerContainer.ServerIoc.Resolve<IProtocolPackage>();
            OnlineManager = ServerContainer.ServerIoc.Resolve<IOnlineManager>();

            //加入内置命令
            var currentaddembly = typeof(ActorService<TOnlineRole>).Assembly;
            var actions = new List<Type>(currentaddembly.GetTypes());
            var ass  = GetActions();
            if(ass!=null)
                actions.AddRange(ass);

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

        private void ProcessSend(object sender, NetMQQueueEventArgs<NetMQMessage> e)
        {
            PushSocket.SendMultipartMessage(e.Queue.Dequeue());
        }

        /// <summary>
        /// 获取额外的接口列表
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Type> GetActions()
        {
            return null;
        }
        /// <summary>
        /// 多线程处理请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessTaskPullSocket(object sender, NetMQSocketEventArgs e)
        {
            var msg = e.Socket.ReceiveMultipartMessage();

            Task.Factory.StartNew(() => ProcessReceive(msg)).ContinueWith(t =>
            {
                //发生内部错误
                if(Logger.IsErrorEnabled)
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
            };
            //执行动作
            var actionname = context.Request.C;
            using (var action = GetActionInstanceByName(actionname))
            {
                if (action != null)
                {
                    //context.Request.Token
                    //获取session信息  从缓存中去获取session信息  session没有的时候需要提示客户端重新连接

                    var onlinerole = OnlineManager.GetOnlineRole(context.SessionId);
                    var token = GetToken(context.Request.Token);
                    if (onlinerole == null)
                    {   //一般在第一次登陆的时候
                        //验证token
                        action.Reslover = ServerContainer.GetDistrictContainer(token.Did).BeginLifetimeScope();
                        if (!VerifyToken(action.Reslover.Resolve<IDistrictConfig>(), token))
                        {
                            //验证不通过
                            HandleException(context, new PirateXException("AuthError", "授权失败"));
                            return;
                        }

                        onlinerole = new OnlineRole()
                        {
                            Id = token.Did,
                            Did = token.Did,
                            StartUtcAt = DateTime.UtcNow,
                            Token = context.Request.Token,
                            SessionId = context.SessionId
                        };

                        ServerContainer.ServerIoc.Resolve<IOnlineManager>().Login(onlinerole);
                    }
                    else //TOKEN 验证过了
                    {
                        action.Reslover = ServerContainer.GetDistrictContainer(token.Did).BeginLifetimeScope();
                    }


                    action.OnlieRole = onlinerole;
                    //TODO action filters
                    action.ServerReslover = ServerContainer.ServerIoc;
                    action.Context = context;
                    action.Logger = Logger;
                    action.MessageSender = this;

                    //try
                    //{
                    //    action.Execute();
                    //}
                    //catch (Exception exception)
                    //{
                    //    HandleException(context, exception);
                    //}

                    action.Execute();
                }
                else
                {
                    var headers = new Dictionary<string, string>
                    {
                        {"c", context.Request.C},
                        {"i", MessageType.Rep},
                        {"o", Convert.ToString(context.Request.O)},
                        {"code", Convert.ToString((int) StatusCode.NotFound)},
                        {"errorCode", "NotFound"},
                        {"errorMsg", $"Action {actionname} not found!"}
                    };
                    //返回类型 

                    SendMessage<string>(context,headers,null);
                }
            }
            
        }

        /// <summary>
        /// 解析token信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual IToken GetToken(string token)
        {
            var odatas = Convert.FromBase64String(token);

            using (var ms = new MemoryStream(odatas))
            {
                return Serializer.Deserialize<Token>(ms);
            }
        }
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="config"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual bool VerifyToken(IDistrictConfig config,IToken token)
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

                errorCode = pe.ErrorCode;
                errorMsg = pe.ErrorMsg;
            }
            else if (e is WebException)
            {
                //code = (short)StatusCode.RemoteError;
                //errorCode = StatusCode.RemoteError.ToString();
                errorMsg = e.Message;
            }
            else
            {
                errorCode = "ServerError";
                errorMsg = "ServerError"; //e.Message;
            }

            if (!(e is PirateXException))
            {
                //if (Logger.IsErrorEnabled)
                //    Logger.Error($"Exception [{ServerId}:{Rid}] - {e.Message} ", e);
            }


            var headers = new Dictionary<string, string>();
            headers.Add("c", context.Request.C);
            headers.Add("i", MessageType.Rep);//返回类型 
            headers.Add("o", Convert.ToString(context.Request.O));
            headers.Add("code", Convert.ToString(code));
            headers.Add("errorCode", errorCode);
            headers.Add("errorMsg", errorMsg);

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

        public void Stop()
        {
            Poller?.Stop();
            PullSocket?.Close();
        }

        #region send message
        public void SendMessage<T>(ActorContext context, T t)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("c", context.Request.C);
            headers.Add("i", MessageType.Rep);//返回类型 
            headers.Add("o", Convert.ToString(context.Request.O));
            headers.Add("code", Convert.ToString((int)StatusCode.Ok));

            SendMessage(context,headers,t);
        }

        public void SendMessage<T>(ActorContext context, string name, T t)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("c", context.Request.C);
            headers.Add("i", MessageType.Boradcast);//通知类型 
            headers.Add("o", Convert.ToString(context.Request.O));
            headers.Add("code", Convert.ToString((int)StatusCode.Ok));

            SendMessage(context, headers, t);
        }

        private void SendMessage<T>(ActorContext context, IDictionary<string, string> header, T rep)
        {
            var repMsg = new NetMQMessage();
            repMsg.Append(new byte[] { context.Version });//版本号
            repMsg.Append("action");//动作
            repMsg.Append(context.SessionId);//sessionid
            repMsg.Append(context.ClientKeys);//客户端密钥
            repMsg.Append(context.ServerKeys);//服务端密钥
            repMsg.Append(GetHeaderBytes(header));//信息头
            if(!Equals(rep,default(T)))
                repMsg.Append(ProtocolPackage.ResponseConvert.SerializeObject(rep));//信息体

            MessageQueue.Enqueue(repMsg);
        }

        private byte[] GetHeaderBytes(IDictionary<string, string> headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.Keys.Select(a => a + "=" + headers[a])));
        }
        #endregion
    }

    static class MessageType
    {
        public const string Rep = "1";

        public const string Boradcast = "2";
    }
}
