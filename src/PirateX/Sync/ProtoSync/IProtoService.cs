using System.Collections.Generic;
using System.Reflection;

namespace PirateX.Sync.ProtoSync
{
    public interface IProtoService
    {
        /// <summary>
        /// 
        /// </summary>
        void Init(Assembly assembly);
        /// <summary>
        /// 获取总的描述
        /// </summary>
        /// <returns></returns>
        string GetProtosHash();
        /// <summary>
        /// 获取模型的hash值
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>
        /// 字典清单，KEY:模型名称,VALUE 
        /// </returns>
        IDictionary<string, string> GetProtosHashDic(Assembly assembly);
    }
}
