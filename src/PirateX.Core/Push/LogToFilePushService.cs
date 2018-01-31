using Newtonsoft.Json;
using NLog;

namespace PirateX.Core
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
