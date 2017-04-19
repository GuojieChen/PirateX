using Autofac;
using PirateX.Core.Online;

namespace PirateX.Core.Actor
{
    public abstract class Login<TResponse> :RepAction<TResponse>
    {
        private static readonly object LoggingHelper = new object();
        public override TResponse Play()
        {
            var onlineManager = base.Reslover.Resolve<IOnlineManager>();

            #region 实行登陆排队，并且排除同一时间同一个账号的可能
            lock (LoggingHelper)
            {
                var onlineInfo = GetOnlineRole();
                onlineManager.Login(onlineInfo);

                //TODO 如果还需要控制登陆数量，可以参考以下网站
                //http://blog.csdn.net/21aspnet/article/details/1539638
            }
            #endregion

            return default(TResponse);
        }


        /// <summary> 获取在线玩家在线信息
        /// </summary>
        /// <returns></returns>
        public abstract IOnlineRole GetOnlineRole();
    }
}
