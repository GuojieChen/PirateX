using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkTest.Domain;
using EntityFrameworkTest.Migrations;

namespace EntityFrameworkTest
{
    public class BlogContext : DbContext
    {
        public DbSet<Users> Users { get; set; }


        public BlogContext(string connectionString):base(connectionString)
        {
            
        }
    }
}
