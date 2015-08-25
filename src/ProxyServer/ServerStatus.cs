using System;

namespace PokemonX.ProxyServer
{
    public class ServerStatus
    {
        public string Id
        {
            get { return string.Format("{0}_{1}", Ip, Port); }
        }
        public string Ip { get; set; }
        public int Port { get; set; }
        /// <summary> 在线人数
        /// </summary>
        public int TotalConnections { get; set; }
        #region 服务器状态信息
        /// <summary> CPU使用量 %
        /// </summary>
        public double CpuUsage { get; set; }
        /// <summary> 可用的工作队列
        /// </summary>
        public int AvailableWorkingThreads { get; set; }
        /// <summary> 可用连接数
        /// </summary>
        public int AvailableCompletionPortThreads { get; set; }
        /// <summary> 最大连接数
        /// </summary>
        public int MaxCompletionPortThreads { get; set; }

        public int MaxWorkingThreads { get; set; }

        public int TotalThreadCount { get; set; }

        /// <summary> 内存使用量
        /// </summary>
        public long MemoryUsage { get; set; }
        /// <summary> 可用发送队列
        /// </summary>
        public int AvialableSendingQueueItems { get; set; }
        /// <summary> 最大可用发送队列
        /// </summary>
        public int TotalSendingQueueItems { get; set; }
        /// <summary> 最大连接数
        /// </summary>
        public int MaxConnectionNumber { get; set; }
        public int RequestHandlingSpeed { get; set; }

        public int TotalHandledRequests { get; set; }

        #endregion
        public Uri GetUri()
        {
            return new Uri(string.Format("ps://{0}:{1}", Ip, Port));
        }
        /// <summary> 最近一次更新时间
        /// </summary>
        public DateTime LastUpdateAt { get; set; }

        public int Create { get; set; }

        public int Login { get; set; }
    }
}
