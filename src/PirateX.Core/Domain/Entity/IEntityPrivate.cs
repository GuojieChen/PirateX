using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Domain.Entity
{
    /// <summary>
    /// 私有数据，归属于某个角色
    /// </summary>
    public interface IEntityPrivate
    {
        /// <summary> 角色ID
        /// </summary>
        long Rid { get; set; }
    }
}
