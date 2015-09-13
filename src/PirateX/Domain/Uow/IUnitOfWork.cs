using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Domain.Entity;
using PirateX.Domain.Repository;

namespace PirateX.Domain.Uow
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
        void QueueCommand(Action<IReadRepository> command);
    }
}
