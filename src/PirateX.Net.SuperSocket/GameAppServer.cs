using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public sealed class GameAppServer:AppServer<ProxySession,BinaryRequestInfo>,INetSend
    {
        private NetService NetService { get; set; }

        public GameAppServer(NetService netService)
        {
            NetService = netService;

            this.NewSessionConnected += new SessionHandler<ProxySession>(appServer_NewSessionConnected);
            this.SessionClosed += new SessionHandler<ProxySession, CloseReason>(appserver_SessionClosed);
            this.NewRequestReceived += new RequestHandler<ProxySession, BinaryRequestInfo>(appServer_NewRequestReceived);
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            NetService.Setup(config.Options.Get("PullSocket"), config.Options.Get("PushSocket"), this);
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

        private void appserver_SessionClosed(ProxySession session, CloseReason value)
        {

        }

        private void appServer_NewSessionConnected(ProxySession session)
        {

        }

        private void appServer_NewRequestReceived(ProxySession session, BinaryRequestInfo requestinfo)
        {
            NetService.ProcessRequest(session.ProtocolPackage,requestinfo.Body);
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
