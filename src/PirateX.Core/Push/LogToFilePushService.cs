using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using PirateX.Core.Redis.StackExchange.Redis.Ex;

namespace PirateX.Core.Push
{
    /// <summary>
    /// 将PUSH内容由NLOG写入到文件中
    /// </summary>
    public class LogToFilePushService:IPushService
    {
        private static readonly Logger Logger = LogManager.GetLogger("push");

        public void Notification(IPushMessage message)
        {
            if (!Logger.IsInfoEnabled)
                return; 

            Logger.Info(JsonConvert.SerializeObject(message));
        }
    }
}
