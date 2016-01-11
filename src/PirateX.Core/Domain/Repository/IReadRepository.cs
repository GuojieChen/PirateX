using System;
using PirateX.Core.Domain.Uow;

namespace PirateX.Core.Domain.Repository
{
    public interface IReadRepository: IDisposable
    {
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
