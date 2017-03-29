using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Container;

namespace PirateX.GameServerTest
{

    public class TestDbContext : DbContext
    {
        public DbSet<Role> Roles { get; set; } 

        public TestDbContext(string connectionString):base(connectionString)
        {
            
        }
    }

    public class TestConfiguration : DbMigrationsConfiguration<TestDbContext>
    {
        public TestConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            
        }

        protected override void Seed(TestDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }

    public class EntityFrameworkDatabaseInitializer : IDatabaseInitializer
    {
        public void Initialize(string connectionString)
        {
            Database.SetInitializer<TestDbContext>(new MigrateDatabaseToLatestVersion<TestDbContext, TestConfiguration>(true));

            using (var db = new TestDbContext(connectionString))
            {
                db.Database.Initialize(true);
            }
        }
    }
}
