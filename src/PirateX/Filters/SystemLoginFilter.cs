using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace PirateX.Filters
{
    public class SystemLoginFilter : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {

        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {
            commandContext.Session.Items[ItemsConst.IsSystem] = true;
        }
    }
}
