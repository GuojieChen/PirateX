using System;
using PirateX.Core.Domain.Repository;

namespace PirateX.Core.Domain.Uow
{
    /// <summary> 工作单元
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary> 提交
        /// </summary>
        void Commit();
        /// <summary> 添加命令
        /// </summary>
        /// <param name="command"></param>
        void QueueCommand(Action<IWriteRepository> command);
    }
}
