using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Domain.Entity;

namespace PirateX.Domain.Uow
{
    /// <summary> 工作单元
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary> 更新
        /// </summary>
        /// <param name="entity"></param>
        void RegisterUpdate(IEntity entity);
        /// <summary> 新增
        /// </summary>
        /// <param name="entity"></param>
        void RegisterAdd(IEntity entity);
        /// <summary> 删除
        /// </summary>
        /// <param name="entity"></param>
        void RegisterRemoved(IEntity entity);
        /// <summary> 提交
        /// </summary>
        void Commit();
    }
}
