using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Container;
using ServiceStack.OrmLite;

namespace PirateX.ServiceStackV3
{
    public class ServiceStackDatabaseInitializer: IDatabaseInitializer
    {
        private IEnumerable<Type> Types { get; set; } 
        public ServiceStackDatabaseInitializer(IEnumerable<Type> types)
        {
            this.Types = types;
        }

        public void Initialize(string connectionString)
        {
            bool IsMysql;
            IDbConnectionFactory dbConnectionFactory;
            IsMysql = IsMySql(connectionString);

            if (IsMysql)
                dbConnectionFactory = new OrmLiteConnectionFactory(connectionString, MySqlDialect.Provider);
            else
                dbConnectionFactory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);

            CreateAndAlterTable(dbConnectionFactory);
        }


        private static readonly string[] MySqlKeys = new[] { "persist security info", "charset", "allow user variables" };

        private static bool IsMySql(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return false;

            var items = connectionString.Split(new char[] { ';' });
            return items.Select(item => item.Split(new char[] { '=' })).Any(ss => MySqlKeys.Contains(ss[0].ToLower()));
        }

        public void CreateAndAlterTable(IDbConnectionFactory dbConnectionFactory)
        {
            using (var db = dbConnectionFactory.OpenDbConnection())
            {
                foreach (var type in Types)
                {
                    if (db.TableExists(type.Name))
                    {
                        if (IsMySql(db.ConnectionString))
                            db.AlterMySqlTable(type);
                        else
                            db.AlterTableSqlServer(type);
                    }
                    else
                    {
                        db.CreateTable(false, type);
                    }
                }
            }
        }
    }
}
