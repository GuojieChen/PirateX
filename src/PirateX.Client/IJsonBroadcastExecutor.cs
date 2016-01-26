namespace PirateX.Client
{
    /// <summary>
    /// 广播数据处理接口
    /// </summary>
    public interface IJsonBroadcastExecutor
    {

    }

    public interface IJsonBroadcastExecutor<in TScoketClient, in TData> : IJsonBroadcastExecutor
        where TScoketClient :PSocketClient
    {
        void Execute(TScoketClient client, TData data); 
    }
}
