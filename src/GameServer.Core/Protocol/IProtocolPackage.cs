using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Package;

namespace GameServer.Core.Protocol
{
    public interface IProtocolPackage<out TRequestInfo>
        where TRequestInfo :IGameRequestInfo
    {
        IPackageProcessor PackageProcessor { get;set; }


        /// <summary>
        /// 序列化包为二进制数据
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] SerializeObject<TMessage>(TMessage message);

        /// <summary>
        /// 解析数据包为请求对象
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        TRequestInfo DeObject(byte[] datas); 
    }

    public interface IProtocolPackage : IProtocolPackage<IGameRequestInfo>
    {
        
    }
}
