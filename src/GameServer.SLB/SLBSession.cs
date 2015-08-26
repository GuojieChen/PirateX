using System;
using System.Net;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.SLB
{
    public class SLBSession : AppSession<SLBSession, BinaryRequestInfo>
    {
        private TcpClientSession _targetSession;

        public new SLBServer AppServer => (SLBServer)base.AppServer;

        public int ReceiveBufferSize { get; set; } = 102400;

        public delegate IServerInfo CreateNewSession();
        public CreateNewSession CreateNewSessionHandler;

        internal void ConnectTarget(IServerInfo serverInfo)
        {
            var targetSession = new AsyncTcpSession(new DnsEndPoint(serverInfo.Ip, serverInfo.Port)) { ReceiveBufferSize = ReceiveBufferSize };
            targetSession.Connected += targetSession_Connected;
            targetSession.Closed += targetSession_Closed;
            targetSession.DataReceived += targetSession_DataReceived;
            targetSession.Error += targetSession_Error;

            targetSession.Connect();
        }

        public void ConnectProxyServer()
        {
            ConnectTarget(CreateNewSessionHandler());
        }


        void targetSession_Error(object sender, ErrorEventArgs e)
        {
            Logger.Error(e.Exception);

            var client = (AsyncTcpSession)sender;

            if (!client.IsConnected)
            {
                this.Close();
            }
        }

        void targetSession_DataReceived(object sender, DataEventArgs e)
        {
            if (!this.Connected)
                return;

            this.Send(e.Data, e.Offset, e.Length);
        }

        void targetSession_Closed(object sender, EventArgs e)
        {
            if (this.Connected)
            {
                _targetSession = null;
                this.Close();
                return;
            }
        }

        void targetSession_Connected(object sender, EventArgs e)
        {
            _targetSession = (AsyncTcpSession)sender;
        }

        internal void RequestDataReceived(byte[] buffer, int offset, int length)
        {
            if (_targetSession == null)
            {
                //重连重发?
                this.Close();
                return;
            }

            _targetSession.Send(buffer, offset, length);
        }

        public void PushRequestToRemoteServer(byte[] buffer, int offset, int length)
        {
            if (_targetSession == null)
            {
                this.Close();
                return;
            }

            _targetSession.TrySend(new ArraySegment<byte>(buffer, offset, length));

            //retry
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            if (_targetSession != null)
            {
                _targetSession.Close();
                _targetSession = null;
            }
        }
    }
}
