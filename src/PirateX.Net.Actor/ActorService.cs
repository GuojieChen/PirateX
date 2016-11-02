using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core.Broadcas;
using PirateX.Core.Container;
using PirateX.Core.Online;
using PirateX.Core.Redis.StackExchange.Redis.Ex;
using PirateX.Net.Actor.Actions;
using PirateX.Protocol.Package;
using StackExchange.Redis;
using Topshelf.Logging;

namespace PirateX.Net.Actor
{
    public class ActorService
    {
        public  static LogWriter Logger = HostLogger.Get<ActorService>();

        private static PullSocket PullSocket { get; set; }
        private static PushSocket PushSocket { get; set; }

        private static NetMQPoller Poller { get; set; }
        private static readonly NetMQQueue<NetMQMessage> MessageQueue = new NetMQQueue<NetMQMessage>();

        private static readonly IDictionary<string, IAction> Actions = new Dictionary<string, IAction>(StringComparer.OrdinalIgnoreCase);

        public static IServerContainer ServerContainer { get; set; }


        private ActorConfig Config { get; }

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


            //IocConfig(builder);
            //ServerContainer.InitContainers(builder);


            var builder = new ContainerBuilder();
            //Redis连接池
            builder.Register(c => ConnectionMultiplexer.Connect(ServerContainer.Settings.RedisHost))
                .As<ConnectionMultiplexer>()
                .SingleInstance();

            //在线管理
            //builder.Register(c => new RedisOnlineManager<TOnlineRole>(c.Resolve<ConnectionMultiplexer>())
            //{
            //    Serializer = c.Resolve<IRedisSerializer>()
            //}).As<IOnlineManager<TOnlineRole>>()
            //  .SingleInstance();

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



            SetupActions(Actions);
        }

        private void ProcessSend(object sender, NetMQQueueEventArgs<NetMQMessage> e)
        {
            PushSocket.SendMultipartMessage(e.Queue.Dequeue());
        }

        public virtual void SetupActions(IDictionary<string, IAction> discoverdActions)
        {
            //加入内置命令
            var currentaddembly = typeof(ActorService).Assembly;
            foreach (var type in currentaddembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (!typeof(IAction).IsAssignableFrom(type))
                    continue;

                var action = ((IAction)Activator.CreateInstance(type));
                discoverdActions.Add(string.IsNullOrEmpty(action.Name) ? type.Name : action.Name, action);
            }
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
                if(Logger.IsErrorEnabled)
                    Logger.Error(t.Exception);
                //发生异常需要处理
                //t.Exception
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        //处理接收
        private static void ProcessReceive(NetMQMessage msg)
        {
            /*
             *  var msg = new NetMQMessage();
                msg.Append(new byte[] { 1 });//版本号
                msg.Append("action");//动作
                msg.Append(session.SessionID);//sessionid
                msg.Append(session.ProtocolPackage.ClientKeys);//客户端密钥
                msg.Append(session.ProtocolPackage.ServerKeys);//服务端密钥
                msg.Append(request.HeaderBytes);//信息头
                msg.Append(request.ContentBytes);//信息体
                //加入队列
             */
            ActorContext context = null;
            try
            {
                context = new ActorContext()
                {
                    Version = msg[0].Buffer[0],
                    ActionName = msg[1].ConvertToString(),
                    SessionId = msg[2].ConvertToString(),
                    ClientKeys = msg[3].Buffer,
                    ServerKeys = msg[4].Buffer,
                    Request = new PirateXRequestInfo(msg[5].Buffer, msg[6].Buffer)
                };
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled) Logger.Error(ex);
                return;
            }
            //执行动作
            var actionname = context.Request.C;
            var action = GetActionInstanceByName(actionname);
            if (action != null)
            {
                //action filters
                action.Context = context;
                action.Logger = Logger;
                action.MessageQeue = MessageQueue;
                action.ProtocolPackage = ServerContainer.ServerIoc.Resolve<IProtocolPackage>();
                action.Execute();
            }
            else
            {

            }
        }

        private static IAction GetActionInstanceByName(string actionname)
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

    }
}
