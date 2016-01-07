using System;
using System.Net;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PirateX.SLB
{
    public class SLBSession : AppSession<SLBSession, BinaryRequestInfo>
    {
        private IServerInfo _serverInfo;
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
            _serverInfo = serverInfo;
        }

        public void ConnectProxyServer()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"ConnectProxyServer");


            var serverinfo = CreateNewSessionHandler();
            if (serverinfo == null)
            {
                this.Close();
            }
            else
            {
                ConnectTarget(serverinfo);

                if (Logger.IsDebugEnabled)
                    Logger.Debug($"Connect to server [{serverinfo.Ip}:{serverinfo.Port}]");
            }
        }


        void targetSession_Error(object sender, ErrorEventArgs e)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"targetSession_Error");

            Logger.Error(e.Exception);

            var client = (AsyncTcpSession)sender;

            if (!client.IsConnected)
            {
                this.Close();
            }
        }

        void targetSession_DataReceived(object sender, DataEventArgs e)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"targetSession_DataReceived");

            if (!this.Connected)
                return;

            this.Send(e.Data, e.Offset, e.Length);

            if (Logger.IsDebugEnabled)
                Logger.Debug($"S2C Send data from {this.RemoteEndPoint} to {_serverInfo.Port}:{_serverInfo.Port}");
        }

        void targetSession_Closed(object sender, EventArgs e)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"targetSession_Closed");

            if (this.Connected)
            {
                this.Close();
            }

            if (_serverInfo != null)
                _serverInfo.ProxyCount--;
        }

        void targetSession_Connected(object sender, EventArgs e)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"targetSession_Connected");

            _targetSession = (AsyncTcpSession)sender;

            if (_serverInfo != null)
                _serverInfo.ProxyCount++;
        }

        public void PushRequestToRemoteServer(byte[] buffer, int offset, int length)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"PushRequestToRemoteServer");

            if (_targetSession == null)
            {
                this.Close();
                return;
            }

            var isSendOk = _targetSession.TrySend(new ArraySegment<byte>(buffer, offset, length));
            if (Logger.IsDebugEnabled)
                Logger.Debug($"C2S Send data from {this.RemoteEndPoint} to {_serverInfo.Ip}:{_serverInfo.Port}\t{isSendOk}");
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"OnSessionClosed");

            if (_targetSession != null)
            {
                _targetSession.Close();
                _targetSession = null;
            }
        }
    }
}