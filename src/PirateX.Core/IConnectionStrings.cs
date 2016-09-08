using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    /// <summary>
    /// 连接字符串集合
    /// </summary>
    public interface IConnectionStrings
    {
        //标识
        string Id { get; set; }

        string ConnectionString { get; set; }
    }
}
