using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    public interface ISyncList<T>
    {
        IList<T> GetList(object rid);
    }

    public abstract class SyncListBase<T> : ISyncList<T> where T : IEntity
       
    {
        private IDbConnection _connection;

        public IList<T> GetList(object rid)
        {
            return null;
        }
    }
}
