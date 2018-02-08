using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core;
using ServiceStack.OrmLite;

namespace PirateX.MySqlConfigReader
{
    public class MySqlConfigProvider : IConfigProvider
    {
        public string Key { get; set; }

        public IEnumerable<T> LoadConfigData<T>() where T : IConfigEntity
        {
            using (var db = new OrmLiteConnectionFactory(Key, MySqlDialect.Provider).OpenDbConnection())
            {
                if (db.TableExists(typeof(T).Name))
                    return db.Select<T>();

                return new T[0];
            }
        }
    }
}
