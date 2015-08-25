using System;
using System.Net;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GameServer.Core.Proxy.V1
{
    public class GameProxySession : AppSession<GameProxySession, BinaryRequestInfo>
    {
        private TcpClientSession m_TargetSession;
        private Action<GameProxySession, TcpClientSession> m_ConnectedAction;


        public new GameProxyServer AppServer => (GameProxyServer)base.AppServer;

        internal void ConnectTarget(Action<GameProxySession, TcpClientSession> connectedAction)
        {
            m_ConnectedAction = connectedAction;
            var targetSession = new AsyncTcpSession(new DnsEndPoint("127.0.0.1",3001)) { ReceiveBufferSize = 2000000 };
            targetSession.Connected += targetSession_Connected;
            targetSession.Closed += targetSession_Closed;
            targetSession.DataReceived += targetSession_DataReceived;
            targetSession.Error += targetSession_Error;

           
            targetSession.Connect();
        }


        public void ConnectRemoteServer()
        {
            ConnectTarget(ProxyConnectedHandle);
        }

        void targetSession_Error(object sender, ErrorEventArgs e)
        {
            Logger.Error(e.Exception);

            var client = (AsyncTcpSession)sender;

            if (!client.IsConnected)
            {
                var connectedAction = m_ConnectedAction;
                m_ConnectedAction = null;
                connectedAction(this, null);
                this.Close();
            }
        }

        void targetSession_DataReceived(object sender, DataEventArgs e)
        {
            if (!this.Connected)
                return;

            try
            {
                this.Send(e.Data, e.Offset, e.Length);
            }
            catch
            {

            }
        }

        void targetSession_Closed(object sender, EventArgs e)
        {
            if (this.Connected)
            {
                m_TargetSession = null;
                this.Close();
                return;
            }
        }

        void targetSession_Connected(object sender, EventArgs e)
        {
            m_TargetSession = (AsyncTcpSession)sender;
            var connectedAction = m_ConnectedAction;
            m_ConnectedAction = null;
            connectedAction(this, m_TargetSession);
        }

        internal void RequestDataReceived(byte[] buffer, int offset, int length)
        {
            if (m_TargetSession == null)
            {
                Logger.Error("Cannot receive data before when target socket is connected");
                this.Close();
                return;
            }


            m_TargetSession.Send(buffer, offset, length);
        }

        public void PushRequestToRemoteServer(byte[] buffer, int offset, int length)
        {
            if (m_TargetSession == null)
            {
                Logger.Error("Cannot receive data before when target socket is connected");
                this.Close();
                return;
            }

            m_TargetSession.TrySend(new ArraySegment<byte>(buffer, offset, length));

            //retry
        }

        private void ProxyConnectedHandle(GameProxySession session, TcpClientSession targetSession)
        {
            //if (targetSession == null)
            //    session.SendResponse(m_FailedResponse, 0, m_FailedResponse.Length);
            //else
            //    session.SendResponse(m_OkResponse, 0, m_OkResponse.Length);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            if (m_TargetSession != null)
            {
                m_TargetSession.Close();
                m_TargetSession = null; 
            }
        }
    }
}
