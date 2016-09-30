using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Container
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringAttribute:Attribute
    {
        public string ConnectionStringId { get; private set; }

        public Type ConnectionProcesser { get; private set; }

        /// <summary>
        /// 连接字符串属性
        /// </summary>
        /// <param name="connectionStringId"></param>
        /// <param name="c"></param>
        /// <param name="connectionProcesser"></param>
        public ConnectionStringAttribute(string connectionStringId, Type connectionProcesser)
        {
            this.ConnectionStringId = connectionStringId;
            this.ConnectionProcesser = connectionProcesser;

            if(!connectionProcesser.IsAssignableFrom(typeof(IConnectionProcesser)))
                throw new ArgumentException("connectionProcesser");
        }
    }

    public interface IConnectionProcesser
    {
        
    }
}
