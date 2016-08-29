using System.Collections.Specialized;

namespace PirateX.Client
{
    public interface IJsonExecutor
    {
        NameValueCollection Header { get; set; }
    }

    /// <summary>
    /// 客户端命令接口
    /// </summary>
    /// <typeparam name="TResponseInfo"></typeparam>
    /// <typeparam name="TSocketClient"></typeparam>
    public interface IJsonExecutor<in TSocketClient, in TResponseInfo> : IJsonExecutor
        where TSocketClient : PirateXClient
    {
        void Excute(TSocketClient client, TResponseInfo data); 
    }
}
