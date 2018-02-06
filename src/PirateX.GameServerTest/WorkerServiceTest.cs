using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Moq;
using NetMQ;
using NetMQ.Sockets;
using NLog;
using NUnit.Framework;
using PirateX.Core;
using PirateX.Protocol;
using ProtoBuf;
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

    public class TestActorService : ActorService<TestActorService>
    {
        public TestActorService(ActorConfig config, IDistrictContainer serverContainer) : base(serverContainer)
        {
        }
    }

    [TestFixture]
    public class WorkerServiceTest
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        IActorService _actorService;

        private PushSocket PushSocket;
        private PullSocket PullSocket;

        private NetMQPoller Poller;

        private Token Token = new Token()
        {
            Rid = 2457,
            Did = 1,
            Sign = "",
            Ts = 123456,
            Uid = "xxxxxx"
        };

        private static int O = 0; 

        private IDistrictContainer ServerContainer = new TestContainer();

        [SetUp]
        public void SetUp()
        {
            PushSocket = new PushSocket("@tcp://*:4556");
            PullSocket = new PullSocket("@tcp://*:4557");

            Poller = new NetMQPoller()
            {
                PushSocket,
                PullSocket
            };

            Poller.RunAsync();


            //var mockContainer = new Mock<DistrictContainer>(new ContainerSetting(), new ServerSetting
            //{
            //    Id = "PirateX.VS-DEV",
            //    RedisHost = "localhost:6379"
            //});


            //mockContainer.Setup(x => x.GetDistrictConfig(It.IsAny<int>()))
            //    .Returns(DistrictConfig);

            //mockContainer.Setup(x => x.LoadDistrictConfigs())
            //    .Returns(new []{ DistrictConfig });

            //mockContainer.Setup(x => x.GetDistrictDatabaseFactory(DistrictConfig))
            //    .Returns(new ServiceStackDatabaseFactory(DistrictConfig.ConnectionString));

            //mockContainer.Setup(x => x.GetConfigDatabaseFactory(DistrictConfig))
            //    .Returns(new ServiceStackDatabaseFactory(DistrictConfig.ConfigConnectionString));

            _actorService = new TestActorService(new ActorConfig()
            {
            }, ServerContainer);

            var districtConfig = ServerContainer.GetDistrictConfig(1);

            Token.Sign = Utils.GetMd5($"{Token.Did}{Token.Rid}{Token.Ts}{districtConfig.SecretKey}");

            _actorService.Start();

            Logger.Debug("debug");
            Logger.Info("debug");
            Logger.Warn("debug");

            //var mock = new Mock<IFoo>();
            //mock.Setup(foo => foo.DoSomething("ping")).Returns(true);



        }

        private string GetToken(Token token)
        {
            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, token);
                bytes = ms.ToArray();
            }

            return HttpUtility.UrlEncode(Convert.ToBase64String(bytes));
        }

        [Test]
        public void setup()
        {
            
        }

        [Test]
        [Repeat(10)]
        public void testconnect()
        {
            var sessionid = Guid.NewGuid().ToString();
            var version = (byte) 1;

            var msg = new NetMQMessage();
            msg.Append(new byte[] { version });//版本号
            msg.Append("action");//动作
            msg.Append(sessionid);//sessionid
            msg.Append(new byte[] { 61, 2, 3 });//客户端密钥
            msg.AppendEmptyFrame();
            msg.Append(Encoding.UTF8.GetBytes($"c=newseed&r=false&o={++O}&uid=xxxxxx&t=123123&token={GetToken(Token)}"));//信息头
            msg.Append(Encoding.UTF8.GetBytes("seed=123123"));//信息体

            PushSocket.SendMultipartMessage(msg);

            NetMQMessage receiveMsg = null;

            PullSocket.TryReceiveMultipartMessage(new TimeSpan(0,0,0,20),ref receiveMsg);

            Console.WriteLine(receiveMsg);

            Assert.IsNotNull(receiveMsg);

            Assert.AreEqual(version, receiveMsg[0].Buffer[0]);
            Assert.AreEqual("action", receiveMsg[1].ConvertToString());
            Assert.AreEqual(sessionid, receiveMsg[2].ConvertToString());
            Assert.GreaterOrEqual(receiveMsg[3].BufferSize, 1);

            var headers = receiveMsg[5].ConvertToString().ToQueryDic();
            Assert.AreEqual(Convert.ToString((int)StatusCode.Ok), headers["code"]);
        }

        [Test]
        public void test_action_not_found()
        {
            var msg = new NetMQMessage();
            msg.Append(new byte[] { 1 });//版本号
            msg.Append("action");//动作
            msg.Append("sessionid");//sessionid
            msg.Append(new byte[] { 14, 2, 3 });//客户端密钥
            msg.Append(new byte[] { 61, 2, 3 });//服务端密钥
            msg.Append(Encoding.UTF8.GetBytes($"c=newseed2&o=123&r=false&t=123123&token={GetToken(Token)}"));//信息头
            msg.Append(Encoding.UTF8.GetBytes("seed=123123"));//信息体

            PushSocket.SendMultipartMessage(msg);

            NetMQMessage receiveMsg = null;
            PullSocket.TryReceiveMultipartMessage(new TimeSpan(0, 0, 0, 20), ref receiveMsg);
            Console.WriteLine(receiveMsg);
            var headers = receiveMsg[5].ConvertToString().ToQueryDic();
            Assert.AreEqual(Convert.ToString((int)StatusCode.NotFound), headers["code"]);
        }

        [Test]
        public void test_exception()
        {
            var sessionid = Guid.NewGuid().ToString();
            var version = (byte)1;

            var msg = new NetMQMessage();
            msg.Append(new byte[] { version });//版本号
            msg.Append("action");//动作
            msg.Append(sessionid);//sessionid
            msg.Append(new byte[] { 14, 2, 3 });//客户端密钥
            msg.Append(new byte[] { 61, 2, 3 });//服务端密钥
            msg.Append(Encoding.UTF8.GetBytes($"c=exceptionaction&o=123&r=false&t=123123&token={GetToken(Token)}"));//信息头
            msg.Append(Encoding.UTF8.GetBytes("seed=123123"));//信息体

            PushSocket.SendMultipartMessage(msg);

            var receiveMsg = PullSocket.ReceiveMultipartMessage();
            Console.WriteLine(receiveMsg);
            var headers = receiveMsg[5].ConvertToString().ToQueryDic();
            Assert.AreEqual(Convert.ToString((int)StatusCode.BadRequest), headers["code"]);
            Assert.AreEqual(ExceptionAction.ErrorCode, headers["errorCode"]);
            Assert.AreEqual(ExceptionAction.ErrorMsg, headers["errorMsg"]);
        }


        [Test]
        [Repeat(10)]
        public void retry()
        {
            var sessionid = Guid.NewGuid().ToString();
            var version = (byte)1;
            var o = TimeUtil.GetTimestamp(DateTime.UtcNow)/10000;

            var msg = new NetMQMessage();
            msg.Append(new byte[] { version });//版本号
            msg.Append("action");//动作
            msg.Append(sessionid);//sessionid
            msg.Append(new byte[0]);//客户端密钥
            msg.Append(new byte[] { 61, 2, 3 });//服务端密钥
            msg.Append(Encoding.UTF8.GetBytes($"c=newseed&r=false&o={o}&t=123123&token={GetToken(Token)}"));//信息头
            msg.Append(Encoding.UTF8.GetBytes("seed=123123"));//信息体
            PushSocket.SendMultipartMessage(msg);
            var receiveMsg = PullSocket.ReceiveMultipartMessage();

            Console.WriteLine(receiveMsg);

            var msg2 = new NetMQMessage();
            msg2.Append(new byte[] { version });//版本号
            msg2.Append("action");//动作
            msg2.Append(sessionid);//sessionid
            msg2.Append(new byte[0]);//客户端密钥
            msg2.Append(new byte[] { 61, 2, 3 });//服务端密钥
            msg2.Append(Encoding.UTF8.GetBytes($"c=newseed&r=true&o={o}&t=123123&token={GetToken(Token)}"));//信息头
            msg2.Append(Encoding.UTF8.GetBytes("seed=123123"));//信息体
            PushSocket.SendMultipartMessage(msg2);
            var receiveMsg2 = PullSocket.ReceiveMultipartMessage();
            Console.WriteLine(receiveMsg2);

            //Assert.AreEqual(receiveMsg.ToString(),receiveMsg2.ToString());
        }

        [Test]
        public void req_and_rep()
        {
            var sessionid = Guid.NewGuid().ToString();
            var version = (byte)1;

            var msg = new NetMQMessage();
            msg.Append(new byte[] { version });//版本号
            msg.Append("action");//动作
            msg.Append(sessionid);//sessionid
            msg.AppendEmptyFrame();
            msg.Append(new byte[] { 61, 2, 3 });//服务端密钥
            msg.Append(Encoding.UTF8.GetBytes($"c=newseed&r=false&o={++O}&t=123123&token={GetToken(Token)}"));//信息头
            msg.Append(Encoding.UTF8.GetBytes("seed=123123"));//信息体

            PushSocket.SendMultipartMessage(msg);

            var receiveMsg = PullSocket.ReceiveMultipartMessage();
            Console.WriteLine(receiveMsg);

            Assert.IsNotNull(receiveMsg);

            Assert.AreEqual(version, receiveMsg[0].Buffer[0]);
            Assert.AreEqual("action", receiveMsg[1].ConvertToString());
            Assert.AreEqual(sessionid, receiveMsg[2].ConvertToString());
            Assert.GreaterOrEqual(receiveMsg[3].BufferSize, 1);

            var headers = receiveMsg[5].ConvertToString().ToQueryDic();
            Assert.AreEqual(Convert.ToString((int)StatusCode.Ok), headers["code"]);
        }


        [TearDown]
        public void TearDown()
        {
            _actorService?.Stop();
            Poller?.Stop();
            PushSocket?.Close();
            PullSocket?.Close();
            Poller?.Dispose();

            PullSocket = null;
            PushSocket = null;
            Poller = null;
            _actorService = null; 

            Thread.Sleep(1000);
        }
    }
}
