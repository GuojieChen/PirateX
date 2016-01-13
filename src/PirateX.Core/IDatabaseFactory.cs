using System;
using System.Collections.Generic;

namespace PirateX.Core
{
    /// <summary>
    /// 数据库的维护操作
    /// </summary>
    public interface IDatabaseFactory
    {
        string ConnectionString { get; }

        /// <summary> 判断表是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool TableExists<T>();
        /// <summary> 表不存在的情况下创建表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void CreateTableIfNotExists<T>();
        /// <summary> 删除表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DropTable<T>();
        /// <summary> 删除并创建表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DropAndCreateTable<T>();
        /// <summary> 更新表结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AlterTable<T>();
        /// <summary> 读取所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<T> Select<T>();
        /// <summary> 创建并且更新表结构
        /// </summary>
        /// <param name="types"></param>
        void CreateAndAlterTable(IEnumerable<Type> types);
    }
}
