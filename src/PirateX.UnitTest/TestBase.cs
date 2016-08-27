﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PirateX.Client;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;

namespace PirateX.UnitTest
{
    [TestFixture]
    public class TestBase
    {

        IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();

        private PSocketClient client;

        [TestFixtureSetUp]
        public void SetUp()
        {

            if (!bootstrap.Initialize())
            {
                Console.WriteLine("Failed to initialize!");
            }

            var result = bootstrap.Start();

            Console.WriteLine("Start result: {0}!", result);

            if (result == StartResult.Success)
            {
                client = new PSocketClient("ps://localhost:3002");
                client.OnError += (sender, args) =>
                {
                    throw new Exception(args.ToString());
                };
                client.OnOpen += (sender, args) =>
                {

                };


                client.Open();
            }

            Thread.Sleep(3000);
        }

        [Test]
        public void Connect()
        {
            Console.WriteLine("OK");
        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            client?.Close();
            bootstrap?.Stop();
        }
    }
}
