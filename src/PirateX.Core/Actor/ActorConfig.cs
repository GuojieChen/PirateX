namespace PirateX.Core
{
    public class ActorConfig
    {
        public string ResponseSocketString { get; set; }

        public string PublisherSocketString { get; set; }
        /// <summary>
        /// 默认初始的后端工作队列数量
        /// </summary>
        public int BackendWorkersPerService { get; set; } = 2;
    }
}
