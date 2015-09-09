using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Domain.Entity;

namespace PirateX.Domain.Uow
{
    public interface IRepository
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        void Save(IEntity entity);
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        void Add(IEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        void Remove(IEntity entity);
        /// <summary> 创建工作单元
        /// </summary>
        /// <returns></returns>
        IUnitOfWork CreateUnitOfWork();
    }


}
