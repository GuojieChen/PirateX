using System.Collections.Generic;
using Autofac;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class ChatService<TMidChatRepository>:ServiceBase
        where TMidChatRepository: MidChatRepository
    {
        public IChat Send(IChat chat)
        {
            return base.Container.ServerIoc.Resolve<TMidChatRepository>().Insert(chat);
        }

        public IEnumerable<TChat> GetLatestChats<TChat>(int channelid) where TChat : IChat
        {
            return base.Container.ServerIoc.Resolve<TMidChatRepository>().GetChats<TChat>(channelid);
        }
    }
}
