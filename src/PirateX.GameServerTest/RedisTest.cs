using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            var redis = ConnectionMultiplexer.Connect("192.168.1.168");

            var db = redis.GetDatabase();
            var tran = db.CreateTransaction();
            tran.SetAddAsync("listtest", "1");
            tran.StringSetAsync("itemtest", "1", new TimeSpan(0, 0, 10, 0));
            tran.KeyExpireAsync("listtest", new TimeSpan(0, 0, 9, 50));
            tran.Execute();
        }

        [Test]
        public void get_from_redis()
        {
            var redis = ConnectionMultiplexer.Connect("192.168.1.168");
            var db = redis.GetDatabase();

            db.SetAdd("listtest", "2");
            db.StringSet("itemtest", "1", new TimeSpan(0, 0, 10, 0));

            //Console.WriteLine(db.StringGet("itemtest"));
            //var list = db.SetMembers("listtest");
            //Console.WriteLine(list.Any());
        }
    }
}
