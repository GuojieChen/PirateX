using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Sync.DataSync
{
    /// <summary>
    /// 标记一个模型为可同步
    /// </summary>
    public class DataSyncAttribute:Attribute 
    {
        public string TableName { get; private set; }


        public DataSyncAttribute(string tablename)
        {
            this.TableName = tablename;
        }
    }
}
