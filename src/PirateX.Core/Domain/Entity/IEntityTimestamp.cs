using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Domain.Entity
{
    /// <summary>
    /// 标记数据需要时间戳来标记数据的变化
    /// </summary>
    public interface IEntityTimestamp
    {
    }
    
    public interface IEntityTimestamp<T>:IEntityTimestamp
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        T Timestamp { get; set; }
    }
}
