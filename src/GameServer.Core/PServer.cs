using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Protocol.PokemonX;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using IGameServer = GameServer.Core.IGameServer;

namespace GameServer.Core
{
    public class PServer<TSession,TErrorCode> : AppServer<TSession, IRequestInfo>, IGameServer
        where TSession : PSession<TSession, TErrorCode>, new()
    {
        public void Broadcast<TMessage>(TMessage message, IQueryable<long> rids)
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
