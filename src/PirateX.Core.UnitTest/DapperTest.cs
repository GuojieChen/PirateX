using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using StackExchange.Redis;

namespace PirateX.Core.UnitTest
{

    public class DefaultConfig
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string V { get; set; }
    }

    public class PetConfig
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AwakeCost { get; set; }

        public int ElementType { get; set; }
    }



    [TestFixture]
    public class DapperTest
    {
        [Test]
        public void select()
        {
            var dapper =
                new SqlConnection(
                    "Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;");
            //using (var dapper = new SqlConnection("Server=192.168.1.213;Database=pirate.core;User ID=pokemonx;Password=123456;Pooling=true;MAX Pool Size=20;Connection Lifetime=10;"))

            for (int i = 0; i < 10; i++)
            {
                var list = dapper.Query<PetConfig>($"select * from {typeof(PetConfig).Name}");
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : {list.Count()}");
            }
        }

        [Test]
        public void redis()
        {
            var db = ConnectionMultiplexer.Connect("192.168.1.216").GetDatabase(2);
            Console.WriteLine(db.KeyExists("j#team_list:2457"));
            Console.WriteLine(db.SetMembers("j#team_list:2457").Count());


        }
    }
}
