using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PirateX.Domain.Entity;
using PirateX.Domain.Uow;

namespace PirateX.Domain.Repository
{
    public interface IRepository
    {
        #region Insert
        /// <summary> 新增
        /// </summary>
        /// <param name="entity"></param>
        void Insert<TEntity>(TEntity entity) where TEntity : IEntity;
        /// <summary> 新增 并且返回自增值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        long Insert<TEntity>(TEntity entity, bool identity) where TEntity : IEntity;

        Task InsertAsync<TEntity>(TEntity entity) where TEntity : IEntity;
        #endregion

        #region Update
        /// <summary> 修改
        /// </summary>
        /// <param name="entity"></param>
        void Update<TEntity>(TEntity entity) where TEntity : IEntity;
        /// <summary> 更新字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="id"></param>
        void Update<TEntity>(object fields, object id);
        /// <summary> 根据条件更新部分字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="predicate"></param>
        void Update<TEntity>(object fields, Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity;
        #endregion

        #region Delete
        /// <summary> 删除
        /// </summary>
        /// <param name="id"></param>
        void Delete<TEntity>(object id) where TEntity : IEntity;
        /// <summary> 删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync<TEntity>(object id) where TEntity : IEntity;
        #endregion

        #region Select

        #endregion

        #region UnitOfWork
        /// <summary> 创建工作单元
        /// </summary>
        /// <returns></returns>
        IUnitOfWork CreateUnitOfWork();
        #endregion
    }


}
