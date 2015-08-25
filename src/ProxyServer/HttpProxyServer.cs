namespace PokemonX.ProxyServer
{
    public class HttpProxyServer : ProxyAppServer
    {
        public HttpProxyServer()
            : base(new HttpProxyReceiveFilterFactory())
        {

        }

        protected override void OnNewSessionConnected(ProxySession session)
        {
            session.Type = ProxyType.Http;
            base.OnNewSessionConnected(session);
        }
    }
}
