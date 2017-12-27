using System.Collections.Generic;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class ChatService<TChat>:ServiceBase
        where TChat : class,IChat
    {
        public TChat Send(TChat chat)
        {
            using (var uow = base.CreateUnitOfWork())
            {
                return uow.Repository<ChatRepository>().Insert<TChat>(chat);
            }
        }

        public IList<TChat> GetLatestChats(long lastid,int size = 50)
        {
            using (var uow = base.CreateUnitOfWork())
            {
                return uow.Repository<ChatRepository>().GetLatestChats<TChat>(lastid,size);
            }
        }
    }
}
