using System.Security.Authentication;
using PirateX.GException;
using PirateX.GException.V1;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace PirateX.Filters
{
    /// <summary> 授权检测
    /// </summary>
    public class AuthorizedFilterBase : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            var islogin = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsLogin);
            if (!islogin)
                throw new GameException(ServerCode.Unauthorized);

            var islogout = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsLogout);
            if (islogout)
                throw new GameException(ServerCode.ReLogin); 

            var isclosed = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsClosed);
            if (isclosed)
                throw new GameException(ServerCode.RoleStoped);
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {

        }
    }
}
