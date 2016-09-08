using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PirateX.Core;
using PirateX.Core.DataSync;

namespace PirateX.Middleware.DataSync
{
    public class DataSync:IDataSync<RoleDatasVersion>,IDisposable
    {
        public IDbConnection DbConnection { get; set; }

        public DataSync()
        {
            DbConnection.Open();
        }

        public IEnumerable<RoleDatasVersion> GetList(long rid, long timestamp)
        {
            return DbConnection.Query<RoleDatasVersion>("SELECT * FROM RoleDatasVersion WHERE RId=@Rid AND Timestamp>@Timestamp",new {Rid=rid, Timestamp=timestamp});
        }

        public void Dispose()
        {
            DbConnection?.Close();
        }
    }
}
