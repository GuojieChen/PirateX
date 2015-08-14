using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.PkProtocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using IGameServer = GameServer.Core.IGameServer;

namespace GameServer.Core
{
    public class PServer<TSession> : AppServer<TSession, ISocketRequestInfo>, IGameServer where TSession : PSession<TSession>, new()
    {
        public PServer()
            : base(new PSocketProtocol())
        {

        }

        /// <summary> 外部指定数据接收过滤器
        /// </summary>
        /// <param name="protocol"></param>
        public PServer(IReceiveFilterFactory<ISocketRequestInfo> protocol)
            : base(protocol)
        {

        }

        public void Broadcast<TMessage>(TMessage message, params long[] rids)
        {
            if (rids == null)
                return;

            try
            {
                var sessions = GetAllSessions().Where(item => rids.Contains(item.Rid));
                foreach (var session in sessions)
                    session.SendMessage(message);
            }
            catch (Exception exception)
            {
                if (Logger.IsErrorEnabled)
                    Logger.Error(exception.Message, exception);
            }
        }
    }
}
