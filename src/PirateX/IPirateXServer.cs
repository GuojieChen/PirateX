using Autofac;
using PirateX.Core;
using PirateX.Protocol;

namespace PirateX
{
    public interface IPirateXServer:IPirateXServerBase
    {
        IServerContainer ServerContainer { get; set; }

        ILifetimeScope ServerIoc { get; }

        #region 请求返回结果的缓存
        void StartRequest(IPirateXSession session, string c);

        void EndRequest(IPirateXSession session, string c, object response);

        bool ExistsReqeust(IPirateXSession session, string c);

        TResponse GetResponse<TResponse>(IPirateXSession session, string c);
        #endregion 
    }
}
