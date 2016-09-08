using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkTest.Domain;
using EntityFrameworkTest.Migrations;

namespace EntityFrameworkTest
{
    public class Programe
    {
        public static void Main(string[] args)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BlogContext, Configuration>("Default"));

            using (var db = new BlogContext())
            {
                db.Database.Initialize(true);
            }
            Console.WriteLine("OK");
            Console.Read();

        }
    }
}
