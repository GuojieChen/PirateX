namespace PirateX.Client
{
    public interface IJsonExecutor
    {
    }

    /// <summary>
    /// 客户端命令接口
    /// </summary>
    /// <typeparam name="TResponseInfo"></typeparam>
    /// <typeparam name="TSocketClient"></typeparam>
    public interface IJsonExecutor<in TSocketClient, in TResponseInfo> : IJsonExecutor
        where TSocketClient : PSocketClient
    {
        void Excute(TSocketClient client, TResponseInfo data); 
    }
}
