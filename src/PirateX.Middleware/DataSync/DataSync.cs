using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using PirateX.Core.DataSync;

namespace PirateX.Middleware
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
