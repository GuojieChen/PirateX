using System;

namespace PirateX.Core
{
    /// <summary>
    /// 公共数据加载和落地接口
    /// </summary>
    public interface IDataSync
    {
        void Load<T>(Func<T> func);

        void Save<T>(Func<T> func);
    }
}
