using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StackExchange.Redis;

namespace PirateX.GameServerTest
{
    [TestFixture]
    public class RedisTest
    {

        [Test]
        public void connect_to_redis()
        {
            var redis = ConnectionMultiplexer.Connect("localhost");


        }
    }
}
