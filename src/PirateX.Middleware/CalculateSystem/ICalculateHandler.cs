namespace PirateX.Middleware
{
    /// <summary>
    /// 用以职责链计算模型
    /// </summary>
    public interface ICalculateHandler<TCalculateContext>
    where TCalculateContext : ICalculateContext
    {
        void Handler(TCalculateContext context);

        void SetNextHandler(ICalculateHandler<TCalculateContext> handler);
    }
}
