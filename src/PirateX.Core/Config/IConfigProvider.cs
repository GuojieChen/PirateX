using System.Collections.Generic;

namespace PirateX.Core
{
    public interface IConfigProvider
    {
        string Key { get;set; }

        IEnumerable<T> LoadConfigData<T>() where T : IConfigEntity;

        //IEnumerable<T> LoadKeyValueConfigData<T>() where T : IConfigKeyValueEntity;
    }
}
