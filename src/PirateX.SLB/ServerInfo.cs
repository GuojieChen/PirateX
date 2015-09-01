namespace PirateX.SLB
{
    public class ServerInfo :IServerInfo
    {
        public object Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public bool Ping { get; set; }
        public int ProxyCount { get; set; }
        public int Weights { get; set; }
    }
}
