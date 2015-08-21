using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Protocol;
using SuperSocket.SocketBase;
using Autofac;
using GameServer.Core.Cointainer;
using GameServer.Core.Online;
using ServiceStack;

namespace GameServer.Core.Command
{
    /// <summary> 角色登陆的抽象包装
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    /// <typeparam name="TLoginRequest"></typeparam>
    /// <typeparam name="TLoginResponse"></typeparam>
    public abstract class Login<TSession,TGameServerConfig,TLoginRequest, TLoginResponse> : GameCommand<TSession, TLoginRequest, TLoginResponse>
        where TSession : PSession<TSession>, IGameSession, new()
        where TLoginRequest :ILoginRequest
        where TLoginResponse :ILoginResponse
        where TGameServerConfig :IGameServerConfig
    {
        protected override TLoginResponse ExecuteResponseCommand(TSession session, TLoginRequest data)
        {
            IToken token = null;
#if DEBUG
            token = new DebugToken()
            {
                Rid = data.Rid,
                ServerId = data.ServerId
            };
#else
            token = UnPackToken(data);
            if (token == null)
            {

            }
#endif

            if (token == null)
            {
                if(Logger.IsFatalEnabled)
                    Logger.Fatal("Token Error.");
                session.Close();
            }

            var appserver = (IGameServer<TGameServerConfig>) session.AppServer;

            session.Build = appserver.GameContainer.GetServerContainer(token.ServerId);

            if (session.Build == null)
            {
                if(Logger.IsFatalEnabled)
                    Logger.Fatal($"Can't find game container\t:\t{token.ServerId}");
                session.Close();
            }
            session.Rid = token.Rid;
            session.ServerId = token.ServerId;

            //TODO 单设备登陆

            var onlineManager = appserver.Container.Resolve<IOnlineManager>();
            var onlineRole = GetOnlineRole(session, data);
            onlineManager.Login(onlineRole);

            if (Logger.IsDebugEnabled)
                Logger.Debug($"Set role online\t:\t{onlineRole.ToJsv()}");
            
            return DoLogin(session,data);
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
        public abstract IOnlineRole GetOnlineRole(TSession session,TLoginRequest request);
        /// <summary> 解析Token包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IToken UnPackToken(TLoginRequest data);
    }

    public class DebugToken : IToken
    {
        public long Rid { get; set; }
        public int ServerId { get; set; }
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
        int ServerId { get; set; }
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
        string Token { get; set; }

        long Rid { get; set; }

        int ServerId { get; set; }
    }

    /// <summary>
    /// 角色登陆返回的抽象
    /// </summary>
    public interface ILoginResponse
    {
        
    }
}
