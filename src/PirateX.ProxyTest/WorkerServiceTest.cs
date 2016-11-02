using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using NUnit.Framework;
using PirateX.Worker;

namespace PirateX.ProxyTest
{
    [TestFixture]
    public class WorkerServiceTest
    {
        WorkerService WorkerService;

        private PushSocket PushSocket; 

        [SetUp]
        public void SetUp()
        {
            PushSocket = new PushSocket("@tcp://*:5556");

            WorkerService = new WorkerService(new WorkerConfig()
            {
                PullConnectHost = ">tcp://localhost:5556"
            },new TestContainer());

            WorkerService.Start();
        }


        [Test]
        public void test()
        {
            Thread.Sleep(1000);
            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append("action");//动作
            msg.Append("sessionid");//sessionid
            msg.Append(new byte[]{14,2,3});//客户端密钥
            msg.Append(new byte[] { 61, 2, 3 });//服务端密钥
            msg.Append(Encoding.UTF8.GetBytes("c=test"));//信息头
            msg.Append(Encoding.UTF8.GetBytes("c=test"));//信息体

            PushSocket.SendMultipartMessage(msg);
        }
    }
}
