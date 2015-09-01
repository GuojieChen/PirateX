using System.Security.Authentication;
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
            var islogin = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsSystem);
            if (!islogin)
                //throw new PException(ServerCode.Unauthorized);
                throw new AuthenticationException("未授权");
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {

        }
    }
}
