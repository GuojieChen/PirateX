using System.Collections.Generic;
using System.Reflection;

namespace PirateX.Core.Actor.ProtoSync
{
    public interface IProtoService
    {
        /// <summary>
        /// 
        /// </summary>
        void Init(List<Assembly> assembly);
        /// <summary>
        /// 获取总的描述
        /// </summary>
        /// <returns></returns>
        string GetProtosHash();
        /// <summary>
        /// 获取protobuf 描述内容
        /// </summary>
        /// <returns></returns>
        string GetProto();
    }
}
