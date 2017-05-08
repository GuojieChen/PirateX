using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Net;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public sealed class GameAppServer:AppServer<ProxySession,BinaryRequestInfo>,INetManager
    {
        public INetService NetService { get; set; }

        public GameAppServer(INetService netService):base(new ProxyReceiveFilterFactory())
        {
            NetService = netService;

        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            NetService.Setup(this);
            return base.Setup(rootConfig, config);
        }

        public override bool Start()
        {
            NetService.Start();
            return base.Start();
        }

        public override void Stop()
        {
            NetService.Stop();
            base.Stop();
        }

        protected override void OnNewSessionConnected(ProxySession session)
        {
            Console.WriteLine($"New Session Connected!{session.SessionID}");
            base.OnNewSessionConnected(session);
        }

        protected override void OnSessionClosed(ProxySession session, CloseReason reason)
        {
            Console.WriteLine($"New Session Closed!{session.SessionID}");

            NetService.OnSessionClosed(GetProtocolPackage(session.SessionID));
            base.OnSessionClosed(session, reason);
        }

        public ProtocolPackage GetProtocolPackage(string sessionid)
        {
            var session = GetSessionByID(sessionid);
            return session?.ProtocolPackage;
        }

        public void Send(string sessionid, byte[] datas)
        {
            var session = GetSessionByID(sessionid);
            session?.Send(datas,0,datas.Length);
        }
    }
}
