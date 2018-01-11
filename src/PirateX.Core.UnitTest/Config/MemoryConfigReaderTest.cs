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

    public class DbConfigProvider:IConfigProvider
    {
        public string Key { get; set; }
        private OrmLiteConnectionFactory dbConnectionFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);

        public void Init()
        {
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
                        Id = $"ID_{i + 1}",
                        Value = $"V_{i + 1}"
                    };

                    db.Insert(kvconfig);
                }
            }
        }

        public IEnumerable<T> LoadConfigData<T>() where T : IConfigEntity
        {
            var catalog = new Catalog();

            List<T> list = new List<T>();
            if (typeof(T).IsAssignableFrom(typeof(TestConfig)))
            {
                for (int i = 0; i < 100000; i++)
                {
                    var t = catalog.CreateInstance<TestConfig>();
                    t.Id = i + 1;
                    t.Lv = i % 5;
                    
                    list.Add((T)Convert.ChangeType(t,typeof(T)));
                }

                for (int i = 0; i < 5; i++)
                {
                    var t = new TestConfig() {Lv = 1, Atk = 10};
                    list.Add((T)Convert.ChangeType(t, typeof(T)));
                }

            }
            else
            {
                for (int i = 0; i < 100000; i++)
                {
                    var kvconfig = new KeyValueTestConfig()
                    {
                        Id = $"ID_{i + 1}",
                        Value = $"V_{i + 1}"
                    };
                    list.Add((T)Convert.ChangeType(kvconfig, typeof(T)));
                }
            }

            return list;

        }
    }

    [TestFixture]
    public class MemoryConfigReaderTest
    {
        private IDbConnectionFactory dbConnectionFactory;

        private MemoryConfigReader MemoryConfigReader;



        [SetUp]
        public void SetUp()
        {
            var dbConfigProvider =  new DbConfigProvider();
            dbConfigProvider.Init();
            Console.WriteLine("init datas....");
            MemoryConfigReader = new MemoryConfigReader(new List<Assembly>(){ typeof(TestConfig).Assembly }, dbConfigProvider);
            
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
        public void unique_index_2()
        {
            var ts = MemoryConfigReader.SingleByIndexes<TestConfig>(new { Lv = 1, Atk = 1 });
            Console.WriteLine("-----");
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
