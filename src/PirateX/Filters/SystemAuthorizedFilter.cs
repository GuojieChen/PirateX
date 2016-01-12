using System.Security.Authentication;
using PirateX.GException;
using PirateX.GException.V1;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace PirateX.Filters
{
    /// <summary> 管理员授权检测
    /// </summary>
    public class SystemAuthorizedFilter : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            var islogin = commandContext.Session.Items.GetValue<bool>(KeyStore.FilterIsSystem);
            if (!islogin)
                throw new GameException(ServerCode.Unauthorized);
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {

        }
    }
}
