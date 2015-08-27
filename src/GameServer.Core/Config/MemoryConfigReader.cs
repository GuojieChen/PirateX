using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Caching;

namespace GameServer.Core.Config
{
    public class MemoryConfigReader : IConfigReader
    {
        private ICacheClient _cacheClient;

        public MemoryConfigReader(ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
        }

        public void Load(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        public T SingleById<T>(object id) where T : IConfigEntity
        {
            throw new NotImplementedException();
        }

        public IList<T> Select<T>() where T : IConfigEntity
        {
            throw new NotImplementedException();
        }

        public TValue GetValue<TKey, TValue>(TKey key)
        {
            throw new NotImplementedException();
        }
    }
}
