using PirateX.GException;
using PirateX.GException.V1;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace PirateX.Filters
{
    /// <summary>
    /// Seed创建动作
    /// </summary>
    public class SeedCreatingFilter : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            var seedCreated = commandContext.Session.Items.GetValue<bool>(KeyStore.FilterSeedCreated);
            if (!seedCreated)
                throw new GameException(ServerCode.PreconditionFailed);
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {

        }
    }
}
