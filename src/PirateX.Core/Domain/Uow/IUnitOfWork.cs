using System;
using NLog.LayoutRenderers.Wrappers;
using PirateX.Core.Domain.Repository;

namespace PirateX.Core.Domain.Uow
{
    /// <summary> 工作单元
    /// </summary>
    public interface IUnitOfWork:IDisposable
    {
        void BeginTrasaction();

        /// <summary> 提交
        /// </summary>
        void Commit();
        /// <summary>
        /// 获取仓储对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Repository<T>() where T : IRepository;
    }
}
