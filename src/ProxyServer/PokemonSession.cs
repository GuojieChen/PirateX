using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using PokemonX.Common.Exception;
using PokemonX.Common.Package;
using PokemonX.StatusCode;
using PokemonX.SuperSocket;
using PokemonX.SuperSocket.Protocol;
using ServiceStack;
using ServiceStack.Redis;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace PokemonX.ProxyServer
{
    public class PokemonSession : PSession<PokemonSession>
    {
        private TcpClientSession m_TargetSession;
        private IPackageProcessor proxyPackageProcessor = new DefaultPackageProcessor();

        public new PokemonServer AppServer{get{return (PokemonServer)base.AppServer;}}

        public EndPoint ServerEndPoint { get; set; }


        private static readonly PooledRedisClientManager PooledRedisClientManager = new PooledRedisClientManager(0, ConfigurationManager.AppSettings["RedisHost"]);

        private static string GetCacheKey(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return "{0}:{1}".Fmt("onlineservers", "game");

            return "{0}:{1}:{2}".Fmt("onlineservers", "game", queueName);
        }

        internal void ConnectTarget(Action<PokemonSession, TcpClientSession> connectedAction)
        {
            var key = GetCacheKey(ConfigurationManager.AppSettings["OnlineQueueName"]);
            IList<ServerStatus> servers = null;
            using (var redis = PooledRedisClientManager.GetClient())
            {
                var keys = redis.GetAllItemsFromList(key);
                servers = redis.GetValues<ServerStatus>(keys);
            }

            var serverStatus = servers.OrderBy(item => item.Create).ThenBy(item => item.Login).FirstOrDefault();
            if (serverStatus == null)
                throw new PException(ServerCode.InternalServerError);

            ServerEndPoint = new DnsEndPoint(serverStatus.Ip, serverStatus.Port);
            
            var targetSession = new AsyncTcpSession(ServerEndPoint);
            targetSession.ReceiveBufferSize = 1024*20;
            targetSession.Connected += targetSession_Connected;
            targetSession.Closed += targetSession_Closed;
            targetSession.DataReceived += targetSession_DataReceived;
            targetSession.Error += targetSession_Error;

            targetSession.Connect();
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

            base.SendJson(proxyPackageProcessor.Unpack(e.Data));
        }

        void targetSession_Closed(object sender, EventArgs e)
        {
            m_TargetSession = null;
            if (this.Connected)
                this.Close();
        }

        void targetSession_Connected(object sender, EventArgs e)
        {
            m_TargetSession = (AsyncTcpSession)sender;
        }

        internal void RequestDataReceived(byte[] buffer, int offset)
        {
            var datas = proxyPackageProcessor.Pack(buffer);

            if (m_TargetSession == null)
            {
                Logger.Error("Cannot receive data before when target socket is connected");
                this.Close();
            }
            else
                m_TargetSession.Send(datas, 0, datas.Length);
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
