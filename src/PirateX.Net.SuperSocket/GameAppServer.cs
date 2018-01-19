using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PirateX.Core.Net;
using PirateX.Protocol.Package;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.Net.SuperSocket
{
    public sealed class GameAppServer : AppServer<ProxySession, BinaryRequestInfo>, INetManager
    {
        private ConcurrentDictionary<int, string> _dic = new ConcurrentDictionary<int, string>();

        public INetService NetService { get; set; }

        public GameAppServer(INetService netService) : base(new ProxyReceiveFilterFactory())
        {
            NetService = netService;

        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            NetService.Setup(this);
            return base.Setup(rootConfig, config);
        }

        private Timer _pingtimer;
        public override bool Start()
        {
            NetService.Start();

            //_pingtimer = new Timer(PingTick, null, 1000 * 60, 1000 * 60);
            return base.Start();
        }

        private void PingTick(object state)
        {
            NetService.Ping(base.GetAllSessions().GroupBy(l => l.Rid).Count());
        }

        public override void Stop()
        {
            NetService.Stop();
            base.Stop();
        }

        protected override void OnNewSessionConnected(ProxySession session)
        {
            base.OnNewSessionConnected(session);
        }

        protected override void UpdateServerStatus(StatusInfoCollection serverStatus)
        {
            base.UpdateServerStatus(serverStatus);
        }

        protected override void OnServerStatusCollected(StatusInfoCollection bootstrapStatus, StatusInfoCollection serverStatus)
        {
            base.OnServerStatusCollected(bootstrapStatus, serverStatus);

            NetService.Ping(base.GetAllSessions().GroupBy(l => l.Rid).Count());
        }


        protected override void OnSessionClosed(ProxySession session, CloseReason reason)
        {
            NetService.OnSessionClosed(GetProtocolPackage(session.Id));

            base.OnSessionClosed(session, reason);
        }

        public IProtocolPackage GetProtocolPackage(string sessionid)
        {
            var session = GetSessionByID(sessionid);
            return session;
        }

        public IProtocolPackage GetProtocolPackage(int rid)
        {
            string sessionid = string.Empty;
            _dic.TryGetValue(rid, out sessionid);

            if (string.IsNullOrEmpty(sessionid))
                return null;

            return GetSessionByID(sessionid);
        }


        public void Attach(IProtocolPackage package)
        {
            _dic.AddOrUpdate(package.Rid, package.Id, ((i, s) => package.Id));
        }

        public void Send(string sessionid, byte[] datas)
        {
            var session = GetSessionByID(sessionid);
            session?.Send(datas, 0, datas.Length);
        }
    }
}
