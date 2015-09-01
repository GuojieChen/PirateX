using PirateX.GException;
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
            var seedCreated = commandContext.Session.Items.GetValue<bool>(ItemsConst.SeedCreated);
            if (!seedCreated)
                //throw new PException(ServerCode.PreconditionFailed);
                throw new PreconditionException();
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {

        }
    }
}
