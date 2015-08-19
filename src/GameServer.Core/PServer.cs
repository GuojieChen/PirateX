using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using IGameServer = GameServer.Core.IGameServer;

namespace GameServer.Core
{
    public class PServer<TSession,TErrorCode> : AppServer<TSession, IGameRequestInfo>, IGameServer
        where TSession : PSession<TSession, TErrorCode>, new()
    {
        public void Broadcast<TMessage>(TMessage message, IQueryable<long> rids)
        {
            if (message == null || rids == null)
                return;

            this.AsyncRun(() =>
            {
                var sessions = GetAllSessions().Where(item => rids.Contains(item.Rid));
                foreach (var session in sessions)
                    session.SendMessage(message);
            }, exception => Logger.Error(exception)); 
        }
    }
}
