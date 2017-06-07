using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Config
{
    public class DbConfigProvider:IConfigProvider
    {
        public string Key { get; set; }

        public IEnumerable<T> LoadConfigData<T>() where T : IConfigIdEntity
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> LoadKeyValueConfigData<T>() where T : IConfigKeyValueEntity
        {
            throw new NotImplementedException();
        }
    }
}
