using System.Collections.Generic;
using Autofac;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class MidChatService<TChat,TChatRepository>:ServiceBase
        where TChatRepository: MidChatRepository<TChat>
        where TChat :IChat
    {
        /// <summary>
        /// 来自程序中系统的聊天
        /// </summary>
        /// <typeparam name="TChat"></typeparam>
        /// <param name="chat"></param>
        /// <returns></returns>
        public virtual TChat SendFromSystem(TChat chat)
        {
            return base.Container.ServerIoc.Resolve<TChatRepository>().Insert(chat);
        }

        /// <summary>
        /// 来自玩家的聊天消息
        /// </summary>
        /// <typeparam name="TChat"></typeparam>
        /// <param name="chat"></param>
        /// <returns></returns>
        public virtual TChat SendFromRole(TChat chat)
        {
            return base.Container.ServerIoc.Resolve<TChatRepository>().Insert(chat);
        }

        public IEnumerable<TChat> GetLatestChats(int channelid) 
        {
            return base.Container.ServerIoc.Resolve<TChatRepository>().GetChats(channelid);
        }
    }
}
