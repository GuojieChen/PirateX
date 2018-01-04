using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Middleware.CalculateSystem
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
