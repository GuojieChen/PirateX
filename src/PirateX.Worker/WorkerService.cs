using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NetMQ;
using NetMQ.Sockets;
using PirateX.Core.Container;
using PirateX.Worker.Actions;
using Topshelf.Logging;

namespace PirateX.Worker
{
    public class WorkerService
    {
        private static LogWriter Logger = HostLogger.Get<WorkerService>();

        private static PullSocket TaskPullSocket { get; set; }

        private static NetMQPoller poller { get; set; }


        private static IDictionary<string, IAction> Actions  = new Dictionary<string, IAction>(StringComparer.OrdinalIgnoreCase);

        public IServerContainer ServerContainer { get; set; }


        private WorkerConfig Config { get;}

        public WorkerService(WorkerConfig config, IServerContainer serverContainer)
        {
            ServerContainer = serverContainer;
            Config = config;
        }

        public void Start()
        {
            Setup();

            poller.RunAsync();
        }

        /// <summary>
        /// 加载各种配置
        /// </summary>
        public virtual void Setup()
        {
            TaskPullSocket = new PullSocket(Config.PullConnectHost);
            TaskPullSocket.ReceiveReady += ProcessTaskPullSocket;
            poller = new NetMQPoller() { TaskPullSocket };

            //var builder = new ContainerBuilder();



            //IocConfig(builder);
            //ServerContainer.InitContainers(builder);

            SetupActions(Actions);
        }

        public virtual void SetupActions(IDictionary<string,IAction> discoverdActions)
        {
            //加入内置命令
            var currentaddembly = typeof (WorkerService).Assembly;
            foreach (var type in currentaddembly.GetTypes())
            {
                if(type.IsInterface)
                    continue;

                if(!type.IsAssignableFrom(typeof(IAction)))
                    continue;

                var action = ((IAction)Activator.CreateInstance(type));
                    discoverdActions.Add(action.Name, action);
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

            Console.WriteLine(msg);

            Task.Factory.StartNew(() => ProcessReceive(msg)).ContinueWith(t =>
            {
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
            WorkerContext context = null;
            try
            {
                context = new WorkerContext()
                {
                    Version = msg[0].Buffer[0],
                    ActionName = msg[1].ConvertToString(),
                    SessionId = msg[2].ConvertToString(),
                    ClientKeys = msg[3].Buffer,
                    ServerKeys = msg[4].Buffer,
                    //Request = new PirateXRequestInfo(msg[5].Buffer, msg[6].Buffer)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (Logger.IsErrorEnabled) Logger.Error(ex);
                return;
            }
            Console.WriteLine(context);
            //执行动作
            //var actionname = context.Request.C;
            //var action = GetActionInstanceByName(actionname);
            //if (action != null)
            //{
            //    //action filters
            //    action.Context = context;
            //    action.Logger = Logger;
            //    action.Execute();
            //}
            //else
            //{

            //}
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
            
        }
    }
}
