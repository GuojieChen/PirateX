using System.Collections.Generic;
using System.Data;
using PirateX.Core.Container;

namespace PirateX.Core.Config
{
    /// <summary> 配置数据读取抽象
    /// </summary>
    public interface IConfigReader
    {
        void Load(IDatabaseFactory connection);
        /// <summary> 根据关键字获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T SingleById<T>(object id) where T :IConfigEntity;
        /// <summary> 获取全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Select<T>() where T : IConfigEntity;

        /// <summary>
        /// KEY - VALUE 结构 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        TValue GetValue<T,TValue>(string key);

        /// <summary> 通过组合索引的方式进行查找 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        T SingleByIndexes<T>(object index) where T : IConfigIdEntity;
    }
}