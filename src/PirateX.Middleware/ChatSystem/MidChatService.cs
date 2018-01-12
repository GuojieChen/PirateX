using System.Collections.Generic;
using Autofac;
using PirateX.Core;

namespace PirateX.Middleware.ChatSystem
{
    public class ChatService:ServiceBase
    {
        public IChat Send(IChat chat)
        {
            return base.Container.ServerIoc.Resolve<ChatRepository>().Insert(chat);
        }

        public IEnumerable<TChat> GetLatestChats<TChat>(int channelid) where TChat : IChat
        {
            return base.Container.ServerIoc.Resolve<ChatRepository>().GetChats<TChat>(channelid);
        }
    }
}
