using System;
using System.Security.Authentication;
using GameServer.Core.GException;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace GameServer.Core.Filters
{
    /// <summary> 授权检测
    /// </summary>
    public class AuthorizedFilterBase : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            //var seedCreated = commandContext.Session.Items.GetValue<bool>(ItemsConst.SeedCreated);
            //if (!seedCreated)
            //    throw new AbstactGameException(ServerCode.Unauthorized);

            var islogin = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsLogin);
            if (!islogin)
                //throw new PException(ServerCode.Unauthorized);
                throw new AuthenticationException();

            var islogout = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsLogout);
            if (islogout)
                //throw new PException(ServerCode.ReLogin);
                throw new LogoutException(); 


            var isclosed = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsClosed);
            if (isclosed)
                //throw new PException(ServerCode.RoleStoped);
                throw new FreezeException();


        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {
        }
    }
}
