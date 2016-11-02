using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Moq;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using NUnit.Framework;
using PirateX.Net.Actor;
using PirateX.Net.Actor.Actions;
using Topshelf.Logging;

namespace PirateX.GameServerTest
{
    public class NLogHostLoggerConfigurator : HostLoggerConfigurator
    {
        public LogWriterFactory CreateLogWriterFactory()
        {
            return (LogWriterFactory)new NLogLogWriterFactory();
        }
    }

    [TestFixture]
    public class WorkerServiceTest
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        ActorService _actorService;

        private PushSocket PushSocket;
        private PullSocket PullSocket;

        private NetMQPoller Poller;

        [SetUp]
        public void SetUp()
        {
            PushSocket = new PushSocket("@tcp://*:5556");
            PullSocket = new PullSocket("@tcp://*:5557");

            Poller = new NetMQPoller()
            {
                PushSocket,
                PullSocket
            };

            Poller.RunAsync();

            _actorService = new ActorService(new ActorConfig()
            {
                PullConnectHost = ">tcp://localhost:5556",
                PushConnectHost = ">tcp://localhost:5557"

            },new TestContainer());

            _actorService.Start();


            Logger.Debug("debug");
            Logger.Info("debug");
            Logger.Warn("debug");

            //var mock = new Mock<IFoo>();
            //mock.Setup(foo => foo.DoSomething("ping")).Returns(true);
        }


        [Test]
        public void test()
        {
            Logger.Debug("debug");
            Logger.Info("debug");
            Logger.Warn("debug");

            Console.WriteLine("debug");
            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append("action");//动作
            msg.Append("sessionid");//sessionid
            msg.Append(new byte[]{14,2,3});//客户端密钥
            msg.Append(new byte[] { 61, 2, 3 });//服务端密钥
            msg.Append(Encoding.UTF8.GetBytes("c=newseed&o=123&r=false&t=123123&token=ddddd"));//信息头
            msg.Append(Encoding.UTF8.GetBytes("seed=123123"));//信息体

            PushSocket.SendMultipartMessage(msg);


            Console.WriteLine("1-----------------------");
            var receiveMsg = PullSocket.ReceiveMultipartMessage();
            Console.WriteLine("2-----------------------");
            Console.WriteLine(receiveMsg);
        }


        [TearDown]
        public void TearDown()
        {
            _actorService.Stop();
            Poller.Stop();
        }
    }
}
