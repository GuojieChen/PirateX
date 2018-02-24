using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using PirateX.Core;
using PirateX.Protocol;

namespace PirateX.Net.NetMQ
{
    public class BackendNetService : IActorNetService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ActorConfig config;

        private PublisherSocket PublisherSocket { get; set; }

        private IActorService _actorService;

        public BackendNetService(IActorService actorService, ActorConfig config)
        {
            this._actorService = actorService;
            _actorService.ActorNetService = this;
            this.config = config;
        }

        private Proxy _broker = null;
        private Proxy _pubBroker = null;
        private INetMQPoller _poller = null;
        private List<Task> ThreadWorkers = new List<Task>();
        private List<ResponseSocket> ResponseSockets = new List<ResponseSocket>();
        private CancellationTokenSource _c_token = new CancellationTokenSource();
        public virtual void Start()
        {
            var connectto = config.ResponseSocketString;
            //在检测到 ResponseSocketString 是已 @开头的 自己内置一个LRUBroker
            if (config.ResponseSocketString.StartsWith("@"))
            {
                var dealer = new DealerSocket("@inproc://backend");
                _broker = new Proxy(new RouterSocket(config.ResponseSocketString), dealer); // new LRUBroker(config.ResponseSocketString, "tcp://*");
                connectto = $">inproc://backend";
                if (Logger.IsTraceEnabled)
                    Logger.Trace($"start inner req/rep proxy");
            }

            var pubConnectTo = config.PublisherSocketString;
            //if (config.PublisherSocketString.StartsWith("@"))
            //{
            //    _pubBroker = new Proxy(new XSubscriberSocket(config.PublisherSocketString), new XPublisherSocket("@inproc://pubbackend"));
            //    pubConnectTo = $">inproc://pubbackend";

            //    if (Logger.IsTraceEnabled)
            //        Logger.Trace($"start inner sub/pub proxy");
            //}

            if (_broker != null)
                Task.Factory.StartNew(_broker.Start);
            if (_pubBroker != null)
                Task.Factory.StartNew(_pubBroker.Start);
            _actorService.Start();

            for (int i = 0; i < config.BackendWorkersPerService; i++)
                ThreadWorkers.Add(Task.Factory.StartNew(WorkerTask, connectto, _c_token.Token));

            PublisherSocket = new PublisherSocket(pubConnectTo);
        }

        private void WorkerTask(object connectTo)
        {
            using (var server = new ResponseSocket(Convert.ToString(connectTo)))
            {
                while (!_c_token.Token.IsCancellationRequested)
                {
                    //server.TryReceiveFrameBytes(out var msg);
                    ThreadProcessRequest(server);
                }
            }
        }

        private void ThreadProcessRequest(NetMQSocket socket)
        {
            byte[] response = null;

            try
            {
                var msg = socket.ReceiveFrameBytes();

                var sw = new Stopwatch();
                sw.Start();
                if (Logger.IsTraceEnabled)
                    Logger.Trace($"ProcessRequest, Thread[{Thread.CurrentThread.ManagedThreadId}] IsThreadPoolThread = {Thread.CurrentThread.IsThreadPoolThread}");

                var din = msg.FromProtobuf<In>();

                if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                    din.Profile.Add("_itin_", $"{DateTime.UtcNow.Ticks}");

                var context = new ActorContext()
                {
                    Version = din.Version, //版本号
                    Action = (byte)din.Action,
                    Request = new PirateXRequestInfo(
                        din.HeaderBytes, //信息头
                        din.QueryBytes) //信息体
                    ,
                    RemoteIp = din.Ip,
                    LastNo = din.LastNo,
                    SessionId = din.SessionId,
                    Profile = din.Profile,
                    ServerName = din.ServerName,
                    FrontendID = din.FrontendID,
                    ServerItmes = din.Items
                };

                response = _actorService.OnReceive(context);

                Logger.Warn($"============={sw.ElapsedMilliseconds}=============");
            }
            catch (Exception exception)
            {
                //TODO 处理，，，
                Logger.Error(exception);
            }
            finally
            {
                //socket.SendMoreFrame(address);
                //socket.SendMoreFrame("");
                if (response != null)
                    socket.SendFrame(response);
                else
                    socket.SendFrame("");
            }
            //TODO 需要处理 finnaly的异常，感知socket 然后重启
        }

        public virtual void Stop()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("ActorNetService Stopping...");

            _c_token.Cancel();
            _broker?.Stop();
            _actorService.Stop();
        }
        /// <summary>
        /// 采用订阅发布的方式,向所有网关推送消息
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        public void PushMessage(string frontendID, NameValueCollection headers, byte[] body)
        {
            PublisherSocket.SendMoreFrame(frontendID).SendFrame(new Out()
            {
                Version = 1,
                Action = PirateXAction.Push,
                LastNo = -1,
                HeaderBytes = GetHeaderBytes(headers),
                BodyBytes = body,

            }.ToProtobuf());
        }

        private byte[] GetHeaderBytes(NameValueCollection headers)
        {
            return Encoding.UTF8.GetBytes(string.Join("&", headers.AllKeys.Select(a => a + "=" + headers[a])));
        }

        public byte[] Seed(ActorContext context, NameValueCollection header, byte cryptobyte, byte[] clientkeys, byte[] serverkeys, byte[] body)
        {
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
                context.Profile.Add("_itout_", $"{DateTime.UtcNow.Ticks}");

            return new Out()
            {
                Version = 1,
                Action = PirateXAction.Seed,
                SessionId = context.SessionId,
                LastNo = context.Request.O,
                HeaderBytes = GetHeaderBytes(header),
                BodyBytes = body,
                Id = context.Token.Rid,

                ClientKeys = clientkeys,
                ServerKeys = serverkeys,
                Crypto = cryptobyte,

                Profile = context.Profile
            }.ToProtobuf();
        }

        public byte[] SendMessage(ActorContext context, NameValueCollection header, byte[] body)
        {
            if (ProfilerLog.ProfilerLogger.IsInfoEnabled)
            {
                if (!context.Profile.ContainsKey("_itout_"))
                    context.Profile.Add("_itout_", $"{DateTime.UtcNow.Ticks}");
            }

            return new Out()
            {
                Version = 1,
                Action = PirateXAction.Req,
                LastNo = context.Request.O,
                HeaderBytes = GetHeaderBytes(header),
                BodyBytes = body,
                Id = context.Token.Rid,
                Profile = context.Profile

            }.ToProtobuf();
        }
    }
}
