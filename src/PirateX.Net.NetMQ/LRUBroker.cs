using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using NLog;

namespace PirateX.Net.NetMQ
{
    /// <summary>
    /// 基于LRU算法的负载均衡器
    /// </summary>
    public class LRUBroker
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private NetMQPoller _poller = new NetMQPoller();
        private ConcurrentQueue<byte[]> workerQueue = new ConcurrentQueue<byte[]>();

        private RouterSocket _frontend;
        private RouterSocket _backend;

        public int BackendPort { get; private set; }

        public static string ReadyString = "READY";

        public LRUBroker(string frontConnectionString, string backendConnectionString)
        {
            _frontend = new RouterSocket(frontConnectionString);
            _backend = new RouterSocket();
            BackendPort = _backend.BindRandomPort(backendConnectionString.TrimStart(new char[] { '@' }));

            _frontend.ReceiveReady += FrontendReceiveReady;
            _backend.ReceiveReady += BackendReceiveReady;

            _poller.Add(_frontend);
            _poller.Add(_backend);
        }

        private void FrontendReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            Logger.Trace($"------------FrontendReceiveReady----------------");

            //front 收到数据
            //client地址
            var clientAddress = e.Socket.ReceiveFrameBytes();
            //空帧
            e.Socket.ReceiveFrameBytes();
            //其他数据
            var msg = e.Socket.ReceiveFrameBytes();
            //从backend中获取一个可用的
            byte[] backendAddress = null;

            if (Logger.IsTraceEnabled)
                Logger.Trace($"LRUBroker.WorkerQueue = {workerQueue.Count}");

            if (workerQueue.TryDequeue(out backendAddress))
            {
                _backend.SendMoreFrame(backendAddress);
                _backend.SendMoreFrame("");
                _backend.SendMoreFrame(clientAddress);
                _backend.SendMoreFrame("");
                _backend.SendFrame(msg);
            }
            else
            {
                if(Logger.IsWarnEnabled)
                    Logger.Warn("No BackendNetService available");
            }
        }

        private void BackendReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            Logger.Trace($"+++++++++++++++++FrontendReceiveReady+++++++++++++++++");

            //  将worker的地址入队
            var address = e.Socket.ReceiveFrameBytes();
            workerQueue.Enqueue(address);
            //  跳过空帧
            e.Socket.ReceiveFrameBytes();
            // 第三帧是“READY”或是一个client的地址
            var clientAddress = e.Socket.ReceiveFrameBytes();
            //  如果是一个应答消息，则转发给client
            if (clientAddress.Length > 1)
            {
                //空帧
                e.Socket.ReceiveFrameBytes();
                var replay = e.Socket.ReceiveFrameBytes();

                _frontend.SendMoreFrame(clientAddress);
                _frontend.SendMoreFrame("");
                _frontend.SendFrame(replay);
            }
            else
            {
                Console.WriteLine($"backend[{Encoding.UTF8.GetString(address)}] receive ready");
            }
        }

        public void Start()
        {
            _poller.RunAsync();
        }

        public void StartAsync()
        {
            _poller.RunAsync();
        }

        public bool IsRunning()
        {
            if (_poller == null)
                return false; 

            return _poller.IsRunning;
        }

        public void Stop()
        {
            _poller.Stop();
        }

        public void StopAsync()
        {
            _poller.StopAsync();
        }
    }
}
