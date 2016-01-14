using System;
using System.Net;
using Autofac;
using PirateX.Protocol;
using Newtonsoft;
using PirateX.Core;
using PirateX.Core.Online;
using StackExchange.Redis;

namespace PirateX.Command
{
    /// <summary> 角色登陆的抽象包装
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    /// <typeparam name="TLoginRequest"></typeparam>
    /// <typeparam name="TLoginResponse"></typeparam>
    /// <typeparam name="TOnlineRole"></typeparam>
    public abstract class Login<TSession, TLoginRequest, TLoginResponse, TOnlineRole> : GameCommand<TSession, TLoginRequest, TLoginResponse>
        where TSession : GameSession<TSession>, IGameSession, new()
        where TLoginRequest : ILoginRequest
        where TLoginResponse : ILoginResponse
        where TOnlineRole : class, IOnlineRole, new()
    {
        protected override TLoginResponse ExecuteResponseCommand(TSession session, TLoginRequest data)
        {
            IToken token = null;
#if DEBUG
            token = new DebugToken()
            {
                Rid = data.Rid,
                DistrictId = data.Did
            };
#else
            token = UnPackToken(data);
#endif

            if (token == null)
            {
                if (Logger.IsFatalEnabled)
                    Logger.Fatal("Token Error.");
                session.Close();
            }

            var appserver = (IGameServer)session.AppServer;

            if (session.Build == null)
                session.Build = appserver.ServerContainer.GetDistrictContainer(token.DistrictId);

            if (session.Build == null)
            {
                if (Logger.IsFatalEnabled)
                    Logger.Fatal($"Can't find game container\t:\t{token.DistrictId}");
                session.Close();
            }
            session.Rid = token.Rid;
            session.ServerId = token.DistrictId;

            var onlineManager = appserver.ServerContainer.ServerIoc.Resolve<IOnlineManager<TOnlineRole>>();

            var oldOnlineInfo = GetOnlineRole(session, data);
            if (oldOnlineInfo != null)
            {//已经登陆，挤下来
                var sub = appserver.ServerContainer.ServerIoc.Resolve<ConnectionMultiplexer>().GetSubscriber();
                sub.Publish(KeyStore.SubscribeChannelLogout, oldOnlineInfo.SessionID, CommandFlags.FireAndForget);
            }

            onlineManager.Login(oldOnlineInfo);

            return DoLogin(session, data);
        }
        /// <summary> 执行游戏的登录逻辑
        /// </summary>
        /// <param name="session"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract TLoginResponse DoLogin(TSession session, TLoginRequest request);
        /// <summary> 获取在线玩家在线信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract TOnlineRole GetOnlineRole(TSession session, TLoginRequest request);
        /// <summary> 解析Token包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IToken UnPackToken(TLoginRequest data);
    }

    public class DebugToken : IToken
    {
        public long Rid { get; set; }
        public int DistrictId { get; set; }
        public long Timestamp { get; set; }
        public string Secret { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    /// <summary> 请求令牌
    /// </summary>
    public interface IToken
    {
        /// <summary> 角色ID
        /// </summary>
        long Rid { get; set; }
        /// <summary> 游戏服ID
        /// </summary>
        int DistrictId { get; set; }
        /// <summary> 时间戳
        /// </summary>
        long Timestamp { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        string Secret { get; set; }
        DateTime? CreateAt { get; set; }
    }

    /// <summary> 角色登陆请求的抽象
    /// </summary>
    public interface ILoginRequest
    {
        /// <summary> TOKEN 令牌
        /// </summary>
        string Token { get; set; }
        /// <summary> RID
        /// </summary>
        long Rid { get; set; }
        /// <summary> 区服ID
        /// </summary>
        int Did { get; set; }
    }

    /// <summary>
    /// 角色登陆返回的抽象
    /// </summary>
    public interface ILoginResponse
    {

    }
}
