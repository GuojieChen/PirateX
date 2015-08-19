using System;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Metadata;

namespace GameServer.Core.Filters
{
    /// <summary>
    /// Seed创建之后的动作
    /// </summary>
    public class SeedCreatedFilter : CommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            var seedCreated = commandContext.Session.Items.GetValue<bool>(ItemsConst.SeedCreated);
            if (seedCreated)
                //throw new PException(ServerCode.SeedReCreate);
                throw new ArgumentException("种子已经创建");
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {
            commandContext.Session.Items[ItemsConst.SeedCreated] = true;

            ((PSession)commandContext.Session).ProtocolPackage.PackageProcessor.CryptoEnable = true;
        }
    }
}
