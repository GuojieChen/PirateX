using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TestDataGenerator;

namespace PirateX.Core.UnitTest.Config
{
    public class PCofnig
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    [TestFixture]
    public class DictionaryPerformance
    {
        public Dictionary<string,string> Dic = new Dictionary<string, string>();
        
        public Dictionary<string,Dictionary<string,string>> Dic2 = new Dictionary<string, Dictionary<string, string>>();
        
        public Dictionary<string,string> Dic22 = new Dictionary<string, string>();


        [SetUp]
        public void SetUP()
        {
            var catalog = new Catalog();

            for (int i = 0; i < 100000; i++)
            {
                var t = catalog.CreateInstance<PCofnig>();
                t.Id = i + 1;

                var urn = GetUrn(t.Id);

                Dic.Add(urn,t.Name);


            }
        }

        private static string GetUrn(int id)
        {
            return $"pconfig_{id}";
        }


        [Test]
        public void all_in_one()
        {
            var sw = new Stopwatch();
            sw.Start();

            var str = Dic[GetUrn(1000)];
            Console.WriteLine(str);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

        }
    }
}
