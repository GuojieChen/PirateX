//using System.Net;
//using SuperSocket.SocketBase;
//using SuperSocket.SocketBase.Protocol;

//namespace PirateX.Protocol.Package
//{
//    public class JsonReceiveFilterFactory : IReceiveFilterFactory<IPirateXRequestInfo>
//    {
//        public IReceiveFilter<IPirateXRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
//        {
//            return new JsonReceiveFilter((IGameSessionBase)appSession);
//        }
//    }
//}