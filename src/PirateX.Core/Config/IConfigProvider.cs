using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Config
{
    public interface IConfigProvider
    {
        string Key { get;set; }

        IEnumerable<T> LoadConfigData<T>() where T : IConfigEntity;

        //IEnumerable<T> LoadKeyValueConfigData<T>() where T : IConfigKeyValueEntity;
    }
}
