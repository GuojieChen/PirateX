namespace PirateX.Middleware.CalculateSystem
{
    public abstract class CalculateHandlerBase<TCalculateContext> :ICalculateHandler<TCalculateContext>
     where TCalculateContext : ICalculateContext
    {
        private ICalculateHandler<TCalculateContext> _nextHandler;
        public void Handler(TCalculateContext context)
        {
            ExecuteHandler(context);

            _nextHandler?.Handler(context);
        }

        protected abstract void ExecuteHandler(TCalculateContext context);

        public void SetNextHandler(ICalculateHandler<TCalculateContext> handler)
        {
            _nextHandler = handler;
        }
    }
}
