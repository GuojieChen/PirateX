using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PirateX.Core.Config;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using TestDataGenerator;

namespace PirateX.Core.UnitTest.Config
{

    [TestFixture]
    public class MemoryConfigReaderTest
    {
        private IDbConnectionFactory dbConnectionFactory;

        private MemoryConfigReader MemoryConfigReader;



        [SetUp]
        public void SetUp()
        {

            Console.WriteLine("init datas....");
            dbConnectionFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);
            MemoryConfigReader = new MemoryConfigReader(new List<Assembly>(){ typeof(TestConfig).Assembly }, () =>
            {
                return dbConnectionFactory.OpenDbConnection();
            });
            var catalog = new Catalog();

            using (var db = dbConnectionFactory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<TestConfig>();
                db.CreateTableIfNotExists<KeyValueTestConfig>();

                for (int i = 0; i < 100000; i++)
                {
                    var t = catalog.CreateInstance<TestConfig>();
                    t.Id = i + 1;
                    t.Lv = i % 5;

                    db.Insert(t);
                }

                for (int i = 0; i < 100000; i++)
                {
                    var kvconfig = new KeyValueTestConfig()
                    {
                        Id = $"ID_{i+1}",
                        V = $"V_{i+1}"
                    };

                    db.Insert(kvconfig);
                }
            }

            MemoryConfigReader.Load();

            Console.WriteLine("init datas ok");
        }


        [Test]
        public void test()
        {
            var t = MemoryConfigReader.SingleById<TestConfig>(1);
            Console.WriteLine(t);
        }

        [Test]
        public void nomalindex_lv_4()
        {
            var sw = new Stopwatch();
            sw.Start();
            var lv = 4;
            var ts = MemoryConfigReader.SelectByIndexes<TestConfig>(new { Lv = lv });

            sw.Stop();
            Console.WriteLine("Count：" + ts.Count());
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void nomalindex_lv_5()
        {
            var sw = new Stopwatch();
            sw.Start();
            var ts = MemoryConfigReader.SelectByIndexes<TestConfig>(new { Lv = 5 });

            sw.Stop();
            Assert.AreEqual(0,ts.Count());
            Console.WriteLine("Count："+ts.Count());
            Console.WriteLine(sw.ElapsedMilliseconds);

        }

        [Test]
        public void unique_index()
        {
            var ts = MemoryConfigReader.SingleByIndexes<TestConfig>(new { Lv = 0,Id = 1 });
            Console.WriteLine(ts);
        }

        [Test]
        public void key_value()
        {
            var v = MemoryConfigReader.GetValue<KeyValueTestConfig, string>("ID_10");
            Console.WriteLine(v);
            Assert.AreEqual("V_10", v);
        }


        [Test]
        public void to_list_and_select()
        {
            var sw = new Stopwatch();
            sw.Start();
            var lv = 4;
            var ts = MemoryConfigReader.Select<TestConfig>().Select(item=>item.Lv == 4);

            sw.Stop();
            Console.WriteLine("Count：" + ts.Count());
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }


}
