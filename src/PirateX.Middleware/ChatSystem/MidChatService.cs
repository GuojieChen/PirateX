using System.Collections.Generic;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class ChatService:ServiceBase
    {
        public IChat Send(IChat chat)
        {
            using (var uow = base.CreateUnitOfWork())
            {
                return uow.Repository<ChatRepository>().Insert(chat);
            }
        }

        public IEnumerable<TChat> GetLatestChats<TChat>(int channelid) where TChat : IChat
        {
            using (var uow = base.CreateUnitOfWork())
            {
                return uow.Repository<ChatRepository>().GetChats<TChat>(channelid);
            }
        }
    }
}
