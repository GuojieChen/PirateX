using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace PirateX.Filters
{
    /// <summary>
    /// 登录成功的操作
    /// </summary>
    public class LoginSuccessFilter : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            //var isLogin = commandContext.Session.Items.GetValue<bool>(ItemsConst.IsLogin);
            //if (isLogin)
            //    throw new AbstactGameException(ServerCode.ReLogin);
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {
            commandContext.Session.Items[KeyStore.FilterIsLogin] = true;
        }
    }
}
