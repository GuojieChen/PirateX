using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Config
{
    /// <summary>
    /// KEY-VALUE 存储模型的接口 
    /// 
    /// id 既Key
    /// </summary>
    public interface IConfigKeyValueEntity :IConfigEntity<string>
    {
        string V { get; set; }
    }
}
