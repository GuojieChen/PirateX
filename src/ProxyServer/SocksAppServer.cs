namespace PokemonX.ProxyServer
{
    public class SocksProxyServer : ProxyAppServer
    {
        public SocksProxyServer()
            : base(new SocksProxyReceiveFilterFactory())
        {

        }
    }
}
