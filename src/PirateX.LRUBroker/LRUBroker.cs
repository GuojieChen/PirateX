﻿using System;
using System.Collections.Concurrent;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace PirateX.LRUBroker
{
    /// <summary>
    /// 基于LRU算法的负载均衡器
    /// </summary>
    public class LRUBroker
    {

        private NetMQPoller _poller = new NetMQPoller();
        private ConcurrentQueue<byte[]> workerQueue = new ConcurrentQueue<byte[]>();

        private RouterSocket _frontend;
        private RouterSocket _backend;

        public int FrontendPort { get; private set; }
        public int BackendPort { get; private set; }

        public LRUBroker(string frontConnectionString, string backendConnectionString)
        {
            _frontend = new RouterSocket(frontConnectionString);
            _backend = new RouterSocket(backendConnectionString);

            _frontend.ReceiveReady += FrontendReceiveReady;
            _backend.ReceiveReady += BackendReceiveReady;

            _poller.Add(_frontend);
            _poller.Add(_backend);
        }

        private void FrontendReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            //front 收到数据
            //client地址
            var clientAddress = e.Socket.ReceiveFrameBytes();
            //空帧
            e.Socket.ReceiveFrameBytes();
            //其他数据
            var msg = e.Socket.ReceiveFrameBytes();
            //从backend中获取一个可用的
            byte[] backendAddress = null;
            if (workerQueue.TryDequeue(out backendAddress))
            {
                _backend.SendMoreFrame(backendAddress);
                _backend.SendMoreFrame("");
                _backend.SendMoreFrame(clientAddress);
                _backend.SendMoreFrame("");
                _backend.SendFrame(msg);
            }
        }

        private void BackendReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            //  将worker的地址入队
            var address = e.Socket.ReceiveFrameBytes();
            workerQueue.Enqueue(address);
            //  跳过空帧
            e.Socket.ReceiveFrameBytes();
            // 第三帧是“READY”或是一个client的地址
            var clientAddress = e.Socket.ReceiveFrameBytes();
            //  如果是一个应答消息，则转发给client
            if (clientAddress.Length>1)
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
            _poller.Run();
        }

        public void StartAsync()
        {
            _poller.RunAsync();
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
