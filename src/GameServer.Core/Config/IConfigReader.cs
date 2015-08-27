using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Caching;
using ServiceStack.Data;

namespace GameServer.Core.Config
{
    /// <summary> 配置数据读取抽象
    /// </summary>
    public interface IConfigReader
    {
        void Load(IDbConnection connection);
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
        IList<T> Select<T>() where T : IConfigEntity;

        /// <summary>
        /// KEY - VALUE 结构 获取值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        TValue GetValue<TKey,TValue>(TKey key);
    }
}
