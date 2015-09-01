namespace PirateX.SLB
{
    /// <summary> 服务器信息抽象
    /// </summary>
    public interface IServerInfo
    {
        /// <summary> Id
        /// </summary>
        object Id { get; set; }
        /// <summary> 名称
        /// </summary>
        string Name { get; set; }
        string Ip { get; set; }
        /// <summary> 监听的端口
        /// </summary>
        int Port { get; set; }
        /// <summary> 是否Ping通
        /// </summary>
        bool Ping { get; set; }
        /// <summary> 代理连接的数量
        /// </summary>
        int ProxyCount { get; set; }
        /// <summary> 权重值
        /// </summary>
        int Weights { get; set; }
    }
}
