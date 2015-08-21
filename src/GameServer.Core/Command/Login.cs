using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Protocol;
using SuperSocket.SocketBase;

namespace GameServer.Core.Command
{
    /// <summary> 角色登陆的抽象包装
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    /// <typeparam name="TLoginRequest"></typeparam>
    /// <typeparam name="TLoginResponse"></typeparam>
    public abstract class Login<TSession,TLoginRequest, TLoginResponse> : GameCommand<TSession, TLoginRequest, TLoginResponse>
        where TSession : PSession<TSession, Enum>, IAppSession<TSession, IGameRequestInfo>, new()
        where TLoginRequest :ILoginRequest
        where TLoginResponse :ILoginResponse
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
            token = UnPackToken(data.Token);
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

            //TODO Server在线统计 
            //TODO 在线状态
            //TODO 单设备登陆


            throw new NotImplementedException();
        }

        /// <summary> 解析Token包
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract IToken UnPackToken(string token);
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
