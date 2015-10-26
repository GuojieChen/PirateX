using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PirateX.Domain.Entity;
using PirateX.Domain.Uow;

namespace PirateX.Domain.Repository
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
