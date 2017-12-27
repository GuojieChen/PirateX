using System;
using System.Collections.Generic;
using PirateX.Core.Domain.Repository;

namespace PirateX.Middleware
{
    public class ChatRepository:RepositoryBase<IChat>
    {
        public TChat Insert<TChat>(IChat chat)
        {
            throw new NotImplementedException();
        }

        public IList<TChat> GetLatestChats<TChat>(long lastid,int size)
            where TChat : IChat
        {
            throw new NotImplementedException();
        }
    }
}
