using PirateX.Core;
using PirateX.Protocol;

namespace PirateX
{
    public interface IGameServer:IGameServerBase
    {
        IServerContainer ServerContainer { get; set; }

        #region 请求返回结果的缓存
        void StartRequest(IGameSession session, string c);

        void EndRequest(IGameSession session, string c, object response);

        bool ExistsReqeust(IGameSession session, string c);

        TResponse GetResponse<TResponse>(IGameSession session, string c);
        #endregion 
    }
}
