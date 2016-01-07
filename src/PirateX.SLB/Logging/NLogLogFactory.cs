using NLog;
using SuperSocket.SocketBase.Logging;

namespace PirateX.SLB.Logging
{
    public class NLogLogFactory : LogFactoryBase
    {
        public NLogLogFactory() : base("NLog.config")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFile">配置文件名称 使用默认构造函数为 NLog.config</param>
        public NLogLogFactory(string configFile) : base(configFile)
        {
            
        }


        public override ILog GetLog(string name)
        {
            return new NLogLog(LogManager.GetLogger(name));
        }
    }
}
